using Telegram.Bot.Types.ReplyMarkups;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class AnswerItem
    {
        public string Message { get; set; }

        public ReplyMarkup Markup { get; set; }
    }
}
