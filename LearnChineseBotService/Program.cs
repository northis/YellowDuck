using System;
using System.Collections.Generic;
using System.ServiceProcess;
using YellowDuck.LearnChineseBotService.LayoutRoot;

namespace YellowDuck.LearnChineseBotService
{
    static class Program
    {
        static void Main()
        {

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
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

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MainFactory.Log.Write("CurrentDomain_UnhandledException", (Exception) e.ExceptionObject,
                new Dictionary<string, object> {{nameof(e.IsTerminating), e.IsTerminating}});

        }
    }
}
