using System;
using System.Configuration;
using Ninject.Modules;
using Telegram.Bot;
using YellowDuck.Common.Logging;
using YellowDuck.LearnChinese.Data.Ef;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChinese.Providers;

namespace YellowDuck.LearnChineseBotService.LayoutRoot
{
    public class LayoutRootConfiguration : NinjectModule
    {
        public override void Load()
        {
            Bind<ISyllableColorProvider>().To<ClassicSyllableColorProvider>();
            Bind<IChineseWordParseProvider>().To<PinyinChineseWordParseProvider>();
            Bind<IStudyProvider>().To<ClassicStudyProvider>();
            Bind<ISyllablesToStringConverter>().To<ClassicSyllablesToStringConverter>();
            Bind<IWordRepository>().To<EfRepository>();
            Bind<IChinesePinyinConverter>().To<Pinyin4NetConverter>();
            Bind<IFlashCardGenerator>().To<WpfFlashCardGenerator>();
            Bind<ILogService>().To<Log4NetService>().InSingletonScope();
            Bind<TelegramBotClient>()
                .ToSelf()
                .WithConstructorArgument(ConfigurationManager.AppSettings["TelegramBotKey"])
                .WithPropertyValue("PollingTimeout", TimeSpan.Parse(ConfigurationManager.AppSettings["PollInterval"]));
        }
    }
}
