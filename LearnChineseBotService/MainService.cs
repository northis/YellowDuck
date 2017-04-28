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
            MainFactory.MainWorker.Start();
        }

        protected override void OnStop()
        {
            MainFactory.MainWorker.Stop();
        }

        public void StartUserInteractive()
        {
            Console.WriteLine("Press Enter to exit");
            OnStart(null);
            Console.ReadLine();
            OnStop();
        }
    }
    
}
