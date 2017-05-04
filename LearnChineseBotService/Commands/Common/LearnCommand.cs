using System.Collections.Generic;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChinese.Data.ObjectModels;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands.Common
{
    public abstract class LearnCommand : NextCommand
    {
        private readonly IStudyProvider _studyProvider;

        protected LearnCommand(IStudyProvider studyProvider)
        {
            _studyProvider = studyProvider;
        }
        
        public override AnswerItem ProcessNext(AnswerItem previousAnswerItem, LearnUnit lUnit)
        {
            var buttons = new List<InlineKeyboardButton[]>();
            foreach (var option in lUnit.Options)
            {
                buttons.Add(new[]
                {
                    new InlineKeyboardButton(option)
                });
            }

            previousAnswerItem.Markup = new InlineKeyboardMarkup { InlineKeyboard = buttons.ToArray() };
            previousAnswerItem.Picture = lUnit.Picture;
            return previousAnswerItem;
        }

        public override AnswerItem ProcessAnswer(AnswerItem previousAnswerItem, MessageItem mItem)
        {
            var checkResult = _studyProvider.AnswerWord(mItem.ChatId, mItem.TextOnly);
            previousAnswerItem.Picture = checkResult.WordStatistic.Word.CardAll;
            previousAnswerItem.Message = (checkResult.Success ? "✅ " : "⛔️ ") + checkResult.WordStatistic;
            previousAnswerItem.Markup = GetLearnMarkup();

            return previousAnswerItem;
        }
    }
}
