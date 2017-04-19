using System;
using System.ServiceProcess;

namespace YellowDuck.LearnChineseBotService
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            if (Environment.UserInteractive)
            {
                var service = new MainService();
                service.StartUserInteractive();
                return;
            }
            var servicesToRun = new ServiceBase[]
            {
                new MainService()
            };

            ServiceBase.Run(servicesToRun);
        }
    }
}
