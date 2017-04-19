using System.IO;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class HelpCommand: CommandBase
    {
        public override AnswerItem Reply(MessageItem mItem)
        {
            var buttons = GetDefaultButtons();

            return new AnswerItem
            {
                Message = "",
                Markup = new ReplyKeyboardMarkup {Keyboard = new[] {buttons}, ResizeKeyboard = true, OneTimeKeyboard = true}
            };
        }


        public override ECommands GetCommandType()
        {
            return ECommands.Help;
        }
    }
}
