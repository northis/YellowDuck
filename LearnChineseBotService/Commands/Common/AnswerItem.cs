using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChinese.Data.ObjectModels;

namespace YellowDuck.LearnChineseBotService.Commands.Common
{
    public class AnswerItem
    {
        public string Message { get; set; }

        public IReplyMarkup Markup { get; set; }

        public GenerateImageResult Picture { get; set; }
    }
}