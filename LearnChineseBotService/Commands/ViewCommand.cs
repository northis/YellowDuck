using System;
using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChinese.Interfaces.Data;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class ViewCommand : CommandBase
    {
        private readonly IWordRepository _repository;

        public ViewCommand(IWordRepository repository)
        {
            _repository = repository;
        }

        public override AnswerItem Reply(MessageItem mItem)
        {
            var answer = new AnswerItem
            {
                Message = string.Empty,
                Markup = new ReplyKeyboardMarkup { Keyboard = new[] { GetDefaultButtons() }, ResizeKeyboard = true, OneTimeKeyboard = false }
            };

            if (string.IsNullOrEmpty(mItem.TextOnly))
            {
                answer.Message = "Введите слово или фразу из словаря для просмотра карточки";
            }
            else
            {
                IWord word;

                try
                {
                    word = _repository.GetWord(mItem.Text);
                }
                catch (Exception e)
                {
                    answer.Message = e.Message;
                    return answer;
                }

                var stat = _repository.GetUserWordStatistic(mItem.UserId, word.Id);

                answer.Message = stat?.ToString() ?? "0";

                answer.Picture = word.CardAll;
            }

            return answer;
        }


        public override ECommands GetCommandType()
        {
            return ECommands.View;
        }
    }
}
