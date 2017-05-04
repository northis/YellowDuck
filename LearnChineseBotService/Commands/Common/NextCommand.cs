using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChinese.Data.ObjectModels;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands.Common
{
    public abstract class NextCommand : CommandBase
    {
        public const string NextCmd = "next";
        
        public override AnswerItem Reply(MessageItem mItem)
        {
            var answerItem = new AnswerItem
            {
                Message = GetCommandIconUnicode()
            };

            try
            {
                if (string.IsNullOrWhiteSpace(mItem.TextOnly) || NextCmd == mItem.TextOnly)
                {
                    var learnUnit = ProcessLearn(mItem);
                    answerItem = ProcessNext(answerItem, learnUnit);
                }
                else
                {
                    answerItem = ProcessAnswer(answerItem, mItem);
                }
            }
            catch (Exception ex)
            {
                answerItem.Message += Environment.NewLine + ex.Message;
            }

            return answerItem;
        }


        public abstract LearnUnit ProcessLearn(MessageItem mItem);
        public abstract AnswerItem ProcessNext(AnswerItem previousAnswerItem, LearnUnit lUnit);
        public abstract AnswerItem ProcessAnswer(AnswerItem previousAnswerItem, MessageItem mItem);

        public IReplyMarkup GetLearnMarkup()
        {
            var mkp = new InlineKeyboardMarkup
            {
                InlineKeyboard = new[]
                {
                    new[] {new InlineKeyboardButton("Next word", NextCmd) }
                }
            };

            return mkp;
        }

        public abstract override string GetCommandIconUnicode();

        public abstract override string GetCommandTextDescription();
    }
}
