using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using com.LandonKey.SocksWebProxy;
using com.LandonKey.SocksWebProxy.Proxy;
using Ninject.Modules;
using Telegram.Bot;
using YellowDuck.Common.Logging;
using YellowDuck.LearnChinese.Data.Ef;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChinese.Providers;
using YellowDuck.LearnChineseBotService.Commands;
using YellowDuck.LearnChineseBotService.Commands.Common;
using YellowDuck.LearnChineseBotService.MainExecution;
using YellowDuck.LearnChineseBotService.WebHook;

namespace YellowDuck.LearnChineseBotService.LayoutRoot
{
    public class LayoutRootConfiguration : NinjectModule
    {
        private static string _currentDir;
        private static string _releaseNotesInfo;

        public static string ReleaseNotesInfo
        {
            get
            {
                if (_releaseNotesInfo != null) return _releaseNotesInfo;

                var path = Path.Combine(CurrentDir, "ReleaseNotes.txt");
                _releaseNotesInfo = File.Exists(path) ? File.ReadAllText(path) : string.Empty;

                return _releaseNotesInfo;
            }
        }

        public static string CurrentDir
        {
            get
            {
                if (_currentDir != null) return _currentDir;

                var thisLocation = Assembly.GetCallingAssembly().Location;
                if (thisLocation == null)
                    _currentDir = Directory.GetCurrentDirectory();

                _currentDir = Path.GetDirectoryName(thisLocation);

                return _currentDir;
            }
        }

        public static DateTime GetDateTime()
        {
            return DateTime.Now;
        }

        public override void Load()
        {
            Bind<ISyllableColorProvider>().To<ClassicSyllableColorProvider>();
            Bind<IChineseWordParseProvider>().To<PinyinChineseWordParseProvider>();
            Bind<IStudyProvider>().To<ClassicStudyProvider>();
            Bind<ISyllablesToStringConverter>().To<ClassicSyllablesToStringConverter>();
            Bind<IWordRepository>()
                .To<EfRepository>()
                .InThreadScope()
                .WithConstructorArgument("useFullText", true);
            Bind<IChinesePinyinConverter>().To<Pinyin4NetConverter>();
            Bind<IFlashCardGenerator>().To<WpfFlashCardGenerator>();
            Bind<ILogService>().To<Log4NetService>().InSingletonScope();


            Bind<AboutCommand>()
                .ToSelf()
                .WithConstructorArgument("releaseNotes", ReleaseNotesInfo);

            Bind<AntiDdosChecker>()
                .ToSelf()
                .InSingletonScope()
                .WithConstructorArgument<Func<DateTime>>(GetDateTime);

            Bind<QueryHandler>()
                .ToSelf()
                .WithConstructorArgument("flashCardUrl", MainFactory.WebhookPublicUrl + "/FlashCard/");

            Bind<WebServer>()
                .ToSelf()
                .InSingletonScope()
                .WithConstructorArgument("webhookUrl", MainFactory.WebhookUrl)
                .WithConstructorArgument("webhookPublicUrl", MainFactory.WebhookPublicUrl)
                .WithConstructorArgument("telegramId", MainFactory.TelegramBotKey)
                .WithConstructorArgument("whControllerName", MainFactory.WebhookControllerName);


            IWebProxy proxy = null;
            if (MainFactory.UseProxy)
            {
                proxy = new SocksWebProxy(new ProxyConfig(
                    //This is an internal http->socks proxy that runs in process
                    IPAddress.Parse("127.0.0.1"),
                    //This is the port your in process http->socks proxy will run on
                    MainFactory.ProxyPort,
                    //This could be an address to a local socks proxy (ex: Tor / Tor Browser, If Tor is running it will be on 127.0.0.1)
                    IPAddress.Parse(MainFactory.ProxyName),
                    //This is the port that the socks proxy lives on (ex: Tor / Tor Browser, Tor is 9150)
                    MainFactory.ProxyPort,
                    //This Can be Socks4 or Socks5
                    ProxyConfig.SocksVersion.Five
                ));
            }

            var tClient =
                new TelegramBotClient(MainFactory.TelegramBotKey, proxy) {Timeout = MainFactory.PollingTimeout};

            Bind<TelegramBotClient>()
                .ToConstant(tClient);

            Bind<HelpCommand>()
                .ToSelf()
                .WithConstructorArgument<Func<CommandBase[]>>(() => MainFactory.VisibleCommandHandlers.Values
                    .ToArray());
            Bind<StartCommand>()
                .ToSelf()
                .WithConstructorArgument<Func<CommandBase[]>>(() => MainFactory.VisibleCommandHandlers.Values
                    .ToArray());

        }
    }
}