using System;
using System.Diagnostics;
using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChineseBotService.Commands.Common;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class AboutCommand: CommandBase
    {
        private readonly string _releaseNotes;

        public AboutCommand(string releaseNotes)
        {
            _releaseNotes = releaseNotes;
        }

        public override AnswerItem Reply(MessageItem mItem)
        {
            var copywrite = "0";

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            if (assembly.Location != null)
            {
                var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                copywrite = fvi.LegalCopyright;
            }

            return new AnswerItem
            {
                Message = $"YellowDuck Learning Chinese Bot{Environment.NewLine}{copywrite}{Environment.NewLine}Contact me: north@live.ru{Environment.NewLine}{_releaseNotes}",
                Markup = new ReplyKeyboardHide()
            };
        }
        
        public override ECommands GetCommandType()
        {
            return ECommands.About;
        }

        public override string GetCommandIconUnicode()
        {
            return "🈴";
        }

        public override string GetCommandTextDescription()
        {
            return "About this bot";
        }
    }
}
