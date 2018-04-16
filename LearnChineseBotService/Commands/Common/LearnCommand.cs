using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChinese.Data.ObjectModels;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands.Common
{
    public abstract class LearnCommand : NextCommand
    {
        public const int MaxAnswerLength = 30;

        private readonly IStudyProvider _studyProvider;

        protected LearnCommand(IStudyProvider studyProvider, EditCommand editCommand) : base(editCommand)
        {
            _studyProvider = studyProvider;
        }

        public override AnswerItem ProcessAnswer(AnswerItem previousAnswerItem, MessageItem mItem)
        {
            var checkResult = _studyProvider.AnswerWord(mItem.ChatId, mItem.TextOnly);
            previousAnswerItem.Picture = checkResult.WordStatistic.Word.CardAll;
            previousAnswerItem.Message = (checkResult.Success ? "✅ " : "⛔️ ") + checkResult.WordStatistic;
            previousAnswerItem.Markup = GetLearnMarkup(checkResult.WordStatistic.Word.Id);

            return previousAnswerItem;
        }

        public override AnswerItem ProcessNext(AnswerItem previousAnswerItem, LearnUnit lUnit)
        {
            var buttons = new List<InlineKeyboardButton[]>();
            foreach (var option in lUnit.Options)
                buttons.Add(new InlineKeyboardButton[]
                {
                    new InlineKeyboardCallbackButton(string.Join("", option.Take(MaxAnswerLength)),"")
                });

            previousAnswerItem.Markup = new InlineKeyboardMarkup {InlineKeyboard = buttons.ToArray()};
            previousAnswerItem.Picture = lUnit.Picture;
            return previousAnswerItem;
        }
    }
}