using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class DefaultCommand: CommandBase
    {
        public override AnswerItem Reply(MessageItem mItem)
        {
            var buttons = GetDictionaryButtons();

            return new AnswerItem
            {
                Message = "Установлена клавиатура по умолчанию",
                Markup = new ReplyKeyboardMarkup {Keyboard = buttons, ResizeKeyboard = true, OneTimeKeyboard = false}
            };
        }


        public override ECommands GetCommandType()
        {
            return ECommands.Default;
        }
        public override string GetCommandDescription()
        {
            return "👌Установить умолчания";
        }
    }
}
