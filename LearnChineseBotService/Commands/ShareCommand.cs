using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class ShareCommand : CommandBase
    {
        public override AnswerItem Reply(MessageItem mItem)
        {
            return new AnswerItem
            {
                Message = "Выберите друга из списка контактов. Внимание! Перед этим он должен добавить бота себе.",
                Markup = new ReplyKeyboardMarkup { Keyboard = GetDictionaryButtons(), ResizeKeyboard = true, OneTimeKeyboard = false }
            };
        }

        public override ECommands GetCommandType()
        {
            return ECommands.Share;
        }

        public override string GetCommandDescription()
        {
            return "Поделиться своим списком слов с другом";
        }
    }
}
