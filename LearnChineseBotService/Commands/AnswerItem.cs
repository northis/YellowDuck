using Telegram.Bot.Types.ReplyMarkups;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class AnswerItem
    {
        public string Message { get; set; }

        public ReplyKeyboardMarkup Markup { get; set; }
    }
}
