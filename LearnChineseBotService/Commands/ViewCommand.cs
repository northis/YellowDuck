using System;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChinese.Interfaces.Data;
using YellowDuck.LearnChineseBotService.Commands.Common;
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
                Message = "👀"
            };

            if (string.IsNullOrEmpty(mItem.TextOnly))
            {
                answer.Message = "Type a chinese word to view it's flash card. Use chinese characters only! Pinyin and translation are not supported!";
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

                answer.Message = stat?.ToString();

                answer.Picture = word.CardAll;
            }

            return answer;
        }


        public override ECommands GetCommandType()
        {
            return ECommands.View;
        }

        public override string GetCommandIconUnicode()
        {
            return "👀";
        }

        public override string GetCommandTextDescription()
        {
            return "View a word's flash card";
        }
    }
}
