using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Ninject;
using YellowDuck.Common.Logging;
using YellowDuck.LearnChineseBotService.Commands;
using YellowDuck.LearnChineseBotService.Commands.Common;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;
using YellowDuck.LearnChineseBotService.WebHook;

namespace YellowDuck.LearnChineseBotService.LayoutRoot
{
    public static class MainFactory
    {
        #region Constructors

        static MainFactory()
        {
            Init();
        }

        #endregion

        #region Properties

        public static StandardKernel NinjectKernel { get; private set; }
        public static PollWorker PollWorker { get; private set; }
        public static WebServer WebServer { get; private set; }
        public static bool UseWebhooks { get; private set; }
        public static Dictionary<ECommands, CommandBase> CommandHandlers { get; private set; }
        public static Dictionary<ECommands, CommandBase> VisibleCommandHandlers { get; private set; }
        public static ILogService Log => NinjectKernel.Get<ILogService>();

        public static string TelegramBotKey = ConfigurationManager.AppSettings["TelegramBotKey"];
        public static TimeSpan PollingTimeout = TimeSpan.Parse(ConfigurationManager.AppSettings["PollingTimeout"]);
        public static string WebhookUrl = ConfigurationManager.AppSettings["WebhookUrl"];
        public static string WebhookPublicUrl = ConfigurationManager.AppSettings["WebhookPublicUrl"];
        public static string WebhookControllerName = "Webhook";

        #endregion

        #region Methods

        public static CommandBase GetCommandHandler(string command)
        {
            var commandEnum = CommandBase.GetCommandType(command);
            return CommandHandlers[commandEnum];
        }

        public static void Init()
        {
            if (NinjectKernel == null)
                NinjectKernel = new StandardKernel(new LayoutRootConfiguration());


            if (PollWorker == null)
                PollWorker = NinjectKernel.Get<PollWorker>();

            if (WebServer == null)
                WebServer = NinjectKernel.Get<WebServer>();

            UseWebhooks = bool.Parse(ConfigurationManager.AppSettings["UseWebhooks"]);

            var handlers = new CommandBase[]
            {
                NinjectKernel.Get<DefaultCommand>(),
                NinjectKernel.Get<ImportCommand>(),
                NinjectKernel.Get<AddCommand>(),
                NinjectKernel.Get<ViewCommand>(),
                NinjectKernel.Get<DeleteCommand>(),
                NinjectKernel.Get<ShareCommand>(),
                NinjectKernel.Get<HelpCommand>(),
                NinjectKernel.Get<StartCommand>(),
                NinjectKernel.Get<LearnWritingCommand>(),
                NinjectKernel.Get<LearnViewCommand>(),
                NinjectKernel.Get<AboutCommand>(),
                NinjectKernel.Get<ModeCommand>(),
                NinjectKernel.Get<LearnSpeakCommand>(),
                NinjectKernel.Get<LearnTranslationCommand>(),
                NinjectKernel.Get<EditCommand>()
            };

            CommandHandlers = handlers.OrderBy(a => a.GetCommandType().ToString())
                .ToDictionary(a => a.GetCommandType(), a => a);

            VisibleCommandHandlers =
                CommandHandlers.Where(
                        a => a.Key != ECommands.Share)
                    .ToDictionary(a => a.Key, a => a.Value);
        }

        #endregion
    }
}