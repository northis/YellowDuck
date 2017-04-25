using System;
using YellowDuck.LearnChineseBotService.Commands.Enums;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class StartCommand : HelpCommand
    {
        public StartCommand(Func<CommandBase[]> getAllComands) : base(getAllComands)
        {
        }

        public override string GetHelpMessage()
        {
            return "Добро пожаловать в наш уютный бот по изучению китайского языка!" + Environment.NewLine + base.GetHelpMessage();
        }


        public override string GetCommandDescription()
        {
            return "🖐Приветствие";
        }

        public override ECommands GetCommandType()
        {
            return ECommands.Start;
        }
    }
}
