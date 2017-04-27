using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class DefaultCommand: CommandBase
    {
        public override AnswerItem Reply(MessageItem mItem)
        {
            return new AnswerItem
            {
                Message = "Установлен режим по умолчанию"
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
