using Telegram.Bot.Types.ReplyMarkups;

namespace YellowDuck.LearnChineseBotService.Commands.Common
{
    public class AnswerItem
    {
        public string Message { get; set; }

        public IReplyMarkup Markup { get; set; }

        public byte[] Picture { get; set; }
    }
}
