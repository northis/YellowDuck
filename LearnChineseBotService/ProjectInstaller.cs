using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace YellowDuck.LearnChineseBotService
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        public ServiceInstaller MainServiceInstaller { get; }

        public ServiceProcessInstaller MainServiceProcessInstaller { get; }

        public ProjectInstaller()
        {
            MainServiceProcessInstaller = new ServiceProcessInstaller();
            MainServiceInstaller = new ServiceInstaller();
            MainServiceProcessInstaller.Account = ServiceAccount.LocalSystem;
            MainServiceInstaller.StartType = ServiceStartMode.Automatic;
            Installers.AddRange(new Installer[]
            {
                MainServiceInstaller,
                MainServiceProcessInstaller
            });
            MainServiceInstaller.Description = "Yellow Duck - Learn Chinese Bot Service";
            MainServiceInstaller.DisplayName = MainServiceInstaller.ServiceName = "YellowDuck.LearnChineseBotService";
        }
    }
}
