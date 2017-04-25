using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class LearnWritingCommand : CommandBase
    {
        public override AnswerItem Reply(MessageItem mItem)
        {
            var buttons = GetLearnButtons();

            return new AnswerItem
            {
                Message = "Установлена клавиатура по умолчанию",
                Markup = new ReplyKeyboardMarkup {Keyboard = buttons, ResizeKeyboard = true, OneTimeKeyboard = false}
            };
        }


        public override ECommands GetCommandType()
        {
            return ECommands.LearnWriting;
        }
        public override string GetCommandDescription()
        {
            return "🖌Учить написание";
        }
    }
}
