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
            return "Welcome to our useful bot! It will help you learn chinese words by memorizing flash cards. You have your personal dictionary, feel free to full it with your own words. You can provide suitable translation to your native language. And this bot will parse and check all the words you gave, highlight tones with colors and check your score in learning words. One 'word' must less than 100 characters, so you can learn even short sentences. It is alpha version of the program, so we will glad if you report us in case of bugs via e-mail in the bot's profile. Enjoy!" + Environment.NewLine + base.GetHelpMessage();
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
