using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;
using System.Net.Mail;

namespace CERNER_Helix_Workflow_Service
{
    public partial class CERNERHelixWorkflowService : ServiceBase
    {
        private Timer _workTimerArchive;
        private Timer _workTimerFlow;
        private Configuration _Configuration;
        bool _ServiceStartupWorkDone = false;
        private object _lockObject = new object();

        public bool VerboseDebug { get; set; }
        public string LogFile { get; set; }

        public CERNERHelixWorkflowService()
        {
            InitializeComponent();
            if (!System.Diagnostics.EventLog.SourceExists("CERNER Helix Workflow Service"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "CERNER Helix Workflow Service", "CERNER Helix Workflow Service Log");
            }
            eventLog1.Source = "CERNER Helix Workflow Service";
            eventLog1.Log = "CERNER Helix Workflow Service Log";
        }

        DataInterface _DataInterface;
        System.IO.FileSystemWatcher _watcher;
        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Raw service start");

            XmlSerializer ser = new XmlSerializer(typeof(Configuration));
            string configFile = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "configuration.xml");
            StreamReader reader = new StreamReader(configFile);
            _Configuration = (Configuration)ser.Deserialize(reader);
            reader.Close();
            string connectionString = "Server=" + _Configuration.DatabaseServer + ";Database=" + _Configuration.DatabaseName + ";User Id=" + _Configuration.DatabaseUsername + ";Password=k&ye_j2ZzmE4^e;";
            _DataInterface = new DataInterface(connectionString);
            _workTimerArchive = new Timer(new TimerCallback(DoWorkArchive), null, _Configuration.TimerInterval * 1000, _Configuration.TimerInterval * 1000);
            _workTimerFlow = new Timer(new TimerCallback(DoWorkFlow), null, _Configuration.TimerInterval * 1000, _Configuration.TimerInterval * 1000);


        }

        void watcher_Created(object sender, FileSystemEventArgs e)
        {

            try
            {
                bool success = _DataInterface.ParseFile(e.FullPath);
                LogEvent("New XML file: " + e.FullPath + ((success) ? " SUCCESS" : " FAILURE"), false);
            }
            catch (Exception ex)
            {
                LogEvent("Exception in flow file watcher event: " + ex.Message + Environment.NewLine + ex.StackTrace, true);
            }


        }

        void MailMessage(string from, string to, string body, string subject, string server, int port)
        {
            MailMessage emailMessage = new MailMessage();
            emailMessage.From = new MailAddress(from);
            foreach (var toAddress in to.Split(new char[] { ',', ';' }))
                emailMessage.To.Add(new MailAddress(toAddress));
            emailMessage.Subject = subject;
            emailMessage.Body = body;
            SmtpClient MailClient = new SmtpClient(server, port);
            try { MailClient.Send(emailMessage); }
            catch (Exception ex)
            {
                eventLog1.WriteEntry("Error - unable to send email: " + ex.Message);
            }
        }

        void DoWorkArchive(object state)
        {

            lock (_lockObject)
                if (!_ServiceStartupWorkDone)
                {
                    _ServiceStartupWorkDone = true;
                    eventLog1.WriteEntry("Service starting");
                    AppDomain currentDomain = AppDomain.CurrentDomain;
                    currentDomain.UnhandledException += new UnhandledExceptionEventHandler(currentDomain_UnhandledException);
                    LogFile = Path.Combine(_Configuration.Root, "log.txt");
                    LogEvent("Service started on machine " + Environment.MachineName + " at " + DateTime.Now.ToLongTimeString(), true);
                    _watcher = new FileSystemWatcher(_Configuration.FlowTubeCheckerRoot, "*.xml");
                    _watcher.IncludeSubdirectories = true;
                    _watcher.Created += new FileSystemEventHandler(watcher_Created);
                    _watcher.EnableRaisingEvents = true;
                }

            ArchiveFiles(_Configuration.Root, _Configuration.ArchiveAfterMinutes);

        }

        void DoWorkFlow(object state)
        {

            lock (_lockObject)
                if (!_ServiceStartupWorkDone)
                    return;

            try
            {
                int UpdateCount = _DataInterface.ParseDirectory(_Configuration.FlowTubeCheckerRoot).Count;
                if (UpdateCount != 0)
                    LogEvent(UpdateCount.ToString() + " flow XML manifests uploaded at " + DateTime.Now.ToLongTimeString(), false);
            }
            catch (Exception ex)
            {
                LogEvent("Exception in DoWorkFlow: " + ex.Message + Environment.NewLine + ex.StackTrace, true);

            }

        }

        public void ArchiveFiles(string root, int ArchiveAfterMinutes)
        {


            //only search files if we are in a working directory


            string currentDirectory = Path.GetFileName(root);
            // eventLog1.WriteEntry("Entering " + root + " directory (" + currentDirectory + ")");

            if (_Configuration.ArchiveAfterMinutesDictionary.ContainsKey(currentDirectory))
            {
                ArchiveAfterMinutes = _Configuration.ArchiveAfterMinutesDictionary[currentDirectory];
                //      eventLog1.WriteEntry("Changing archive after setting to " + c.ArchiveAfterMinutesDictionary[currentDirectory].ToString() + " for directory " + currentDirectory);
            }

            if (currentDirectory == _Configuration.WorkingDirectoryName || _Configuration.WorkingDirectoryName == "")
            {
                if (_Configuration.ArchiveAfterMinutesDictionary.ContainsKey(currentDirectory))
                    ArchiveAfterMinutes = _Configuration.ArchiveAfterMinutesDictionary[currentDirectory];

                DirectoryInfo di = new DirectoryInfo(root);
                foreach (string f in Directory.GetFiles(root))
                    if (File.GetCreationTime(f) < DateTime.Now.AddMinutes(-ArchiveAfterMinutes) && f != LogFile)
                    {
                        string filenameMain = Path.GetFileNameWithoutExtension(f);
                        string filenameExt = Path.GetExtension(f);
                        string directoryWorking = Path.GetDirectoryName(f);
                        string directoryArchive = Path.Combine(directoryWorking, _Configuration.ArchiveDirectoryName);
                        directoryArchive = Path.Combine(directoryArchive, DateTime.Now.ToString("MM_dd_yyyy"));
                        if (!Directory.Exists(directoryArchive))
                        {
                            Directory.CreateDirectory(directoryArchive);
                            LogEvent("Directory created: " + directoryArchive, true);
                        }
                        string destinationNameBase = Path.Combine(directoryArchive, filenameMain);
                        int suffix = 1;
                        string destinationName = destinationNameBase + filenameExt;
                        //check for name collisions, if found try numeric suffixes up to 99, after that we'll go to exception handling
                        while (File.Exists(destinationName) && suffix <= 99)
                        {
                            destinationName = destinationNameBase + "_dup" + suffix.ToString() + filenameExt;
                            suffix++;
                        }
                        try
                        {
                            File.Move(f, destinationName);
                            LogEvent("File move completed: " + f + " moved to " + destinationName, false);

                        }
                        catch (Exception ex) { LogEvent("Error moving file " + f + System.Environment.NewLine + ex.Message, false); }
                    }
            }

            //recurse tree
            foreach (string d in Directory.GetDirectories(root))
                if (Path.GetFileName(d) != _Configuration.ArchiveDirectoryName)
                    ArchiveFiles(d, ArchiveAfterMinutes);


        }

        void currentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            eventLog1.WriteEntry("Unhandled service exception: " + e.ExceptionObject.ToString());
            MailMessage(_Configuration.EmailFrom, _Configuration.NotificationEmailAddresses, "Unhandled service exception in CERNER Helix Workflow Service Message: " + e.ExceptionObject.ToString() + System.Environment.NewLine + "On " + Environment.MachineName + " at " + DateTime.Now.ToLongTimeString(), "CERNER Helix Workflow Service Error", _Configuration.SMTPServer, _Configuration.SMTPPort);
            Exception ex = e.ExceptionObject as Exception;
            LogDatabaseMessage("Unhandled service exception: " + ex.Message, "Error", ex.StackTrace);
        }

        protected override void OnStop()
        {
            _watcher.EnableRaisingEvents = false;
            _workTimerFlow.Dispose();
            _workTimerArchive.Dispose();
            LogEvent("Service stopping on machine " + Environment.MachineName + " at " + DateTime.Now.ToLongTimeString(), true);
        }


        public void LogEvent(string message, bool SendEmail)
        {
            eventLog1.WriteEntry(message);
            if (SendEmail)
                MailMessage(_Configuration.EmailFrom, _Configuration.NotificationEmailAddresses, message, "CERNER Helix Workflow Service Message", _Configuration.SMTPServer, _Configuration.SMTPPort);
            //  File.AppendAllText(this.LogFile, message + System.Environment.NewLine);
            try
            {
                _DataInterface.InsertEventLog(message, "Untyped Message", "", Environment.MachineName, "Service");
            }
            catch (Exception ex)
            {

                eventLog1.WriteEntry(ex.Message);
                if (SendEmail)
                    MailMessage(_Configuration.EmailFrom, _Configuration.NotificationEmailAddresses, ex.Message, "CERNER Helix Workflow Service Message", _Configuration.SMTPServer, _Configuration.SMTPPort);


            }

        }


        public void LogDatabaseMessage(string EventMessage, string EventType, string EventStack = "")
        {
            try
            {
                _DataInterface.InsertEventLog(EventMessage, EventType, EventStack, Environment.MachineName, "Service");
            }
            catch  
            {
            }


        }



        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }




    }
}
