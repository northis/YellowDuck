using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class CleanCommand: CommandBase
    {
        public override AnswerItem Reply(MessageItem mItem)
        {
            return new AnswerItem
            {
                Message = "Сброс выполнен",
                Markup = new ReplyKeyboardHide()
            };
        }

        public override ECommands GetCommandType()
        {
            return ECommands.Clean;
        }

        public override string GetCommandDescription()
        {
            return "Убрать все кнопки";
        }
    }
}
