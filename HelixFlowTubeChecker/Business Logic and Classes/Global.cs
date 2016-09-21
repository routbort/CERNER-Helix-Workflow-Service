   using System;
    using System.Configuration;
    using System.Collections.Generic;

    public static class Global
    {
     //   public static ApplicationSettings APPLICATION_SETTINGS;
        public static string SAMPLE_CONNECTION = "";
        public static bool IS_TEST_SYSTEM = false;


        public static string CURRENT_USERNAME { get; set; }

        public static string CONNECTION_PROD
        {
            get
            {
                return ConfigurationManager.AppSettings["CONNECTION_PROD"];
            }
        }

        public static string CONNECTION_TEST
        {
            get
            {
                return ConfigurationManager.AppSettings["CONNECTION_TEST"];
            }
        }


        public static string DIRECTORY
        {
            get
            {
                return ConfigurationManager.AppSettings["DIRECTORY"];
            }
        }


        public static string AD_GROUP
        {
            get
            {
                return ConfigurationManager.AppSettings["AD_GROUP"];
            }
        }

      
        public static List<int> USB_SCANNER_CONFIG
        {
            get
            {
                List<int> output = new List<int>();
                foreach (string setting in ConfigurationManager.AppSettings["USB_SCANNER_CONFIG"].Split(new char[] { ',' }))
                    output.Add(Convert.ToInt32(setting));
                return output;

            }
        }

       


    }
 