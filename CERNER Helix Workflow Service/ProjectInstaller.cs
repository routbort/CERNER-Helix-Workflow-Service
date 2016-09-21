using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;

namespace CERNER_Helix_Workflow_Service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            this.BeforeInstall += new InstallEventHandler(ProjectInstaller_BeforeInstall);
        }

        void ProjectInstaller_BeforeInstall(object sender, InstallEventArgs e)
        {

            List<ServiceController> services = new List<ServiceController>(ServiceController.GetServices());

            foreach (ServiceController s in services)
            {
                if (s.ServiceName == this.serviceInstaller1.ServiceName)
                {
                    ServiceInstaller ServiceInstallerObj = new ServiceInstaller();
                    ServiceInstallerObj.Context = new System.Configuration.Install.InstallContext();
                    ServiceInstallerObj.Context = Context;
                    ServiceInstallerObj.ServiceName = "CERNER Helix Workflow Service";
                    ServiceInstallerObj.Uninstall(null);
                }
            }

        }
    }
}
