using System.Collections.Generic;
using System.Linq;
using Ninject;
using YellowDuck.Common.Logging;
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

            if (MainWorker == null)
                MainWorker = NinjectKernel.Get<MainWorker>();

            var handlers = new CommandBase[]
            {
                NinjectKernel.Get<DefaultCommand>(),
                NinjectKernel.Get<ImportCommand>(),
                NinjectKernel.Get<AddCommand>(),
                NinjectKernel.Get<CleanCommand>(),
                NinjectKernel.Get<ViewCommand>(),
                NinjectKernel.Get<DeleteCommand>()
            };

            CommandHandlers = handlers.ToDictionary(a => a.GetCommandType(), a => a);
        }

        #endregion
    }
}
