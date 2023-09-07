using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CERNER_Helix_Workflow_Service
{
    [Serializable]
    public class ConfigKeyValue
    {
        public string Key { get; set; }
        public int Value { get; set; }
    }


    [Serializable()]
    public class Configuration
    {
        public string Root { get; set; }
        public string FlowTubeCheckerRoot { get; set; }
        public string FlowTubeCheckerRootBeaker { get; set; }
        public string DatabaseServer { get; set; }
        public string DatabaseName { get; set; }
        public string DatabaseUsername { get; set; }
        public string WorkingDirectoryName { get; set; }
        public string ArchiveDirectoryName { get; set; }
        public string NotificationEmailAddresses { get; set; }
        public string SMTPServer { get; set; }
        public int SMTPPort { get; set; }
        public string EmailFrom { get; set; }
        public int ArchiveAfterMinutes { get; set; }
        public int TimerInterval { get; set; }
 

        [XmlArray]
        [XmlArrayItem(ElementName = "ArchiveAfterMinutesEntry")]
        public List<ConfigKeyValue> ArchiveAfterMinutesList { get; set; }


        [XmlIgnore]
        public Dictionary<string, int> ArchiveAfterMinutesDictionary
        {
            get { return ArchiveAfterMinutesList.ToDictionary(x => x.Key, x => x.Value); }
            set { ArchiveAfterMinutesList = value.Select(x => new ConfigKeyValue() { Key = x.Key, Value = x.Value }).ToList(); }
        }




        public Configuration()
        {

        }
    }
}
