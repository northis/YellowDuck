using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceProcess;
using System.Threading;
using YellowDuck.LearnChineseBotService.LayoutRoot;

namespace YellowDuck.LearnChineseBotService
{
    internal static class Program
    {
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MainFactory.Log.Write("CurrentDomain_UnhandledException", (Exception) e.ExceptionObject,
                new Dictionary<string, object> {{nameof(e.IsTerminating), e.IsTerminating}});
            Environment.Exit(-1);
        }

        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

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