using System;
using System.Collections.Generic;
using System.Configuration;
using Ninject;
using Telegram.Bot;
using YellowDuck.Common.Logging;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChineseBotService.Commands;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

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
        public static MainWorker MainWorker { get; private set; }

        public static Dictionary<ECommands, CommandBase> CommandHandlers { get; private set; }
        public static ILogService Log => NinjectKernel.Get<ILogService>();

        public const string CommandStartChar = "/";

        #endregion

        #region Methods

        public static CommandBase GetCommandHandler(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command), "Команда не может быть пустой");

            if (!command.StartsWith(CommandStartChar))
                throw new ArgumentException($"Команда должна начинаться с символа '{CommandStartChar}'", nameof(command));

            var cleanedCommand = command.Substring(1, command.Length - 1);

            ECommands commandEnum;
            if(!Enum.TryParse(cleanedCommand, true, out commandEnum))
                throw new NotSupportedException($"Команда '{ nameof(command)}' не поддерживается");

            return CommandHandlers[commandEnum];
        }

        public static void Init()
        {
            if (NinjectKernel == null)
                NinjectKernel = new StandardKernel(new LayoutRootConfiguration());

            if (MainWorker == null)
                MainWorker =
                    new MainWorker(new TelegramBotClient(ConfigurationManager.AppSettings["TelegramBotKey"])
                    {
                        PollingTimeout = TimeSpan.Parse(ConfigurationManager.AppSettings["PollInterval"])
                    }, NinjectKernel.Get<IStudyProvider>(), NinjectKernel.Get<IWordRepository>());


            CommandHandlers = new Dictionary<ECommands, CommandBase>
            {
                {
                    ECommands.Help, new HelpCommand()
                },
                {
                    ECommands.Import,
                    new ImportCommand(NinjectKernel.Get<IChineseWordParseProvider>(),
                        NinjectKernel.Get<IWordRepository>(), NinjectKernel.Get<IFlashCardGenerator>())
                }
            };

        }

        #endregion
    }
}
