using System;
using System.ServiceProcess;
using YellowDuck.LearnChineseBotService.LayoutRoot;

namespace YellowDuck.LearnChineseBotService
{
    public partial class MainService : ServiceBase
    {
        public MainService()
        {
            InitializeComponent();
            MainFactory.Init();
        }

        protected override void OnStart(string[] args)
        {
            if (MainFactory.UseWebhooks)
                MainFactory.WebServer.Start();
            else
                MainFactory.PollWorker.Start();
        }

        protected override void OnStop()
        {
            if (MainFactory.UseWebhooks)
                MainFactory.WebServer.Stop();
            else
                MainFactory.PollWorker.Stop();
        }

        public void StartUserInteractive()
        {
            Console.WriteLine("Press any key to quit");

            OnStart(null);
            Console.ReadKey();
            OnStop();
        }
    }

}
