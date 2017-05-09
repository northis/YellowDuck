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
            return $"Welcome to Chinese Duck Bot! {Environment.NewLine}It will help you learn chinese words by memorizing flash cards. {Environment.NewLine}You have your personal dictionary, feel free to fill it with your own words. You can provide suitable translation on your native language. And this bot will parse and check all the words you gave, highlight tones with colors and check your score in learning words. One 'word' must be less than 15 characters, so you can learn even short sentences. {Environment.NewLine}It is alpha version of the program, so I will be glad if you contact me @DeathWhinny in case of bugs. Happy studying!{Environment.NewLine}{Environment.NewLine}List of available commands:{Environment.NewLine}{base.GetHelpMessage()}" ;
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
