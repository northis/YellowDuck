using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChineseBotService.Commands.Common;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class DeleteCommand : CommandBase
    {
        public const string YesAnswer = "yes";
        public const string NoAnswer = "no";
        private readonly IWordRepository _repository;

        public DeleteCommand(IWordRepository repository)
        {
            _repository = repository;
        }

        public override string GetCommandIconUnicode()
        {
            return "🗑";
        }

        public override string GetCommandTextDescription()
        {
            return "Remove a word from the dictionary";
        }


        public override ECommands GetCommandType()
        {
            return ECommands.Delete;
        }

        public override AnswerItem Reply(MessageItem mItem)
        {
            IReplyMarkup markup = null;

            string message;

            if (string.IsNullOrEmpty(mItem.TextOnly))
            {
                message =
                    "Type a chinese word to remove it from the dictionary. All word's score information will be removed too!";
            }
            else if (NoAnswer == mItem.TextOnly.ToLowerInvariant())
            {
                message = "Delete has been cancelled";
            }
            else if (mItem.TextOnly.ToLowerInvariant().StartsWith(YesAnswer))
            {
                try
                {
                    var word = _repository.GetWord(mItem.TextOnly.Replace(YesAnswer, string.Empty));
                    _repository.DeleteWord(word.Id);

                    message = $"Word {word.OriginalWord} has been removed";
                }
                catch (Exception e)
                {
                    message = e.Message;
                }
            }
            else
            {
                message = $"Do you really want to remove '{mItem.TextOnly}'?";
                markup = new InlineKeyboardMarkup
                {
                    InlineKeyboard = new InlineKeyboardButton[][]
                    {
                        new[]
                        {
                            new InlineKeyboardCallbackButton("✅Yes", $"yes{mItem.TextOnly}"),
                            new InlineKeyboardCallbackButton("❌No", "no")
                        }
                    }
                };
            }

            var answer = new AnswerItem
            {
                Message = message,
                Markup = markup
            };

            return answer;
        }
    }
}