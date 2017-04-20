using System;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class DeleteCommand : CommandBase
    {
        private readonly IWordRepository _repository;

        public const string YesAnswer = "yes";
        public const string NoAnswer = "no";

        public DeleteCommand(IWordRepository repository)
        {
            _repository = repository;
        }

        public override AnswerItem Reply(MessageItem mItem)
        {
            IReplyMarkup markup = new ReplyKeyboardMarkup
            {
                Keyboard = GetDefaultButtons(),
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };

            string message;

            if (string.IsNullOrEmpty(mItem.TextOnly))
            {
                message = "Введите слово или фразу из словаря для удаления. Будет стёрта также и статистика изучения!";
            }
            else if (NoAnswer == mItem.TextOnly.ToLowerInvariant())
            {
                message = "Удаление отменено";
            }
            else if(mItem.TextOnly.ToLowerInvariant().StartsWith(YesAnswer))
            {
                try
                {
                    var word = _repository.GetWord(mItem.TextOnly.Replace(YesAnswer, string.Empty));
                    _repository.DeleteWord(word.Id);

                    message = $"Слово {word.OriginalWord} удалено";
                }
                catch (Exception e)
                {
                    message = e.Message;
                }
            }
            else
            {
                message = $"Вы действительно хотите удалить слово '{mItem.TextOnly}'?";
                markup = new InlineKeyboardMarkup
                {
                    InlineKeyboard = new[] { new[] { new InlineKeyboardButton("Да", $"yes{mItem.TextOnly}"), new InlineKeyboardButton("Нет","no") } }
                };//.Keyboard = new[] {new[] {new KeyboardButton("Да"), new KeyboardButton("Нет")}};
            }

            var answer = new AnswerItem
            {
                Message = message,
                Markup = markup
            };

            return answer;
        }


        public override ECommands GetCommandType()
        {
            return ECommands.Delete;
        }
    }
}
