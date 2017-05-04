using System;
using YellowDuck.LearnChineseBotService.Commands.Common;
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
            return "Welcome to our useful bot! It will help you learn chinese words by memorizing flash cards. You have your personal dictionary, feel free to full it with your own words. You can provide suitable translation to your native language. Enjoy!" + Environment.NewLine + base.GetHelpMessage();
        }
        
        public override ECommands GetCommandType()
        {
            return ECommands.Start;
        }


        public override string GetCommandIconUnicode()
        {
            return "🖐";
        }

        public override string GetCommandTextDescription()
        {
            return "Welcome";
        }
    }
}
