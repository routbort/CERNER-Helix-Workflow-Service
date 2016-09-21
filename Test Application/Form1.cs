using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CERNER_Helix_Workflow_Service;
using System.Xml.Serialization;
using System.IO;

namespace Test_Application
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Configuration c = new Configuration();
            c.Root = @"C:\test";
            c.ArchiveDirectoryName = "Archived Files";
            c.WorkingDirectoryName = "Working Directory";
            c.SMTPPort = 25;
            c.SMTPServer = "mail.mdanderson.org";
            c.EmailFrom = "noreply@mdanderson.org";
            c.NotificationEmailAddresses = "mark.routbort@mdanderson.org";
            c.ArchiveAfterMinutesDictionary = new Dictionary<string, int> { { "A", 23 }, { "B", 14 } };

            XmlSerializer ser = new XmlSerializer(typeof(Configuration));
            TextWriter writer = new StreamWriter(@"C:\temp\configuration.xml");
            ser.Serialize(writer, c);
            writer.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Configuration c = new Configuration();
            XmlSerializer ser = new XmlSerializer(typeof(Configuration));
            string configFile = @"C:\temp\configuration.xml";
            StreamReader reader = new StreamReader(configFile);
            c = (Configuration)ser.Deserialize(reader);
            reader.Close();
            
        }
    }
}
