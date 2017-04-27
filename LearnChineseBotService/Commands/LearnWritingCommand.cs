using System;
using System.Collections.Generic;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class LearnWritingCommand : CommandBase
    {
        private readonly IStudyProvider _studyProvider;
        public const string NextCmd = "next";

        public LearnWritingCommand(IStudyProvider studyProvider)
        {
            _studyProvider = studyProvider;
        }
        public override AnswerItem Reply(MessageItem mItem)
        {
            var answerItem = new AnswerItem
            {
                Message = "🖌",
            };

            try
            {
                if (string.IsNullOrWhiteSpace(mItem.TextOnly) || NextCmd == mItem.TextOnly)
                {
                    var newWord = _studyProvider.LearnWord(mItem.ChatId, ELearnMode.OriginalWord,
                        EGettingWordsStrategy.NewMostDifficult);

                    var buttons = new List<InlineKeyboardButton[]>();
                    foreach (var option in newWord.Options)
                    {
                        buttons.Add(new[]
                        {
                            new InlineKeyboardButton(option)
                        });
                    }

                    answerItem.Markup = new InlineKeyboardMarkup { InlineKeyboard = buttons.ToArray() };
                    answerItem.Picture = newWord.Picture;

                }
                else
                {
                    var checkResult = _studyProvider.AnswerWord(mItem.ChatId, mItem.TextOnly);
                    answerItem.Picture = checkResult.WordStatistic.Word.CardAll;
                    answerItem.Message = (checkResult.Success ? "✅ " : "⛔️ ") + checkResult.WordStatistic;
                    answerItem.Markup = GetLearnMarkup();
                }
            }
            catch (Exception ex)
            {
                answerItem.Message += Environment.NewLine + ex.Message;
            }

            return answerItem;
        }

        public virtual IReplyMarkup GetLearnMarkup()
        {
            var mkp = new InlineKeyboardMarkup
            {
                InlineKeyboard = new[]
                {
                    new[] {new InlineKeyboardButton("🖌Дальше", NextCmd) },
                    new[] {new InlineKeyboardButton("", $"{CommandStartChar}{ECommands.Help}")}
                }
            };

            return mkp;
        }

        public override ECommands GetCommandType()
        {
            return ECommands.LearnWriting;
        }
        public override string GetCommandDescription()
        {
            return "🖌Учить написание";
        }
    }
}
