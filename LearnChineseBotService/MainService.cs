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

        }

        protected override void OnStop()
        {
        }

        public void StartUserInteractive()
        {
            Console.WriteLine("Нажмите любую клавишу для выхода");
            OnStart(null);
            Console.ReadKey();
        }
    }
}
