using System;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChineseBotService.Commands.Common;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class DefaultCommand: CommandBase
    {
        private readonly IWordRepository _repository;

        public DefaultCommand(IWordRepository repository)
        {
            _repository = repository;
        }

        public override AnswerItem Reply(MessageItem mItem)
        {
            try
            {
                _repository.SetLearnMode(mItem.ChatId, EGettingWordsStrategy.OldMostDifficult);
            }
            catch (Exception e)
            {
                return new AnswerItem
                {
                    Message = e.Message
                };
            }
            return new AnswerItem
            {
                Message = "Defaut mode has been set (hard, old words first)"
            };
        }


        public override ECommands GetCommandType()
        {
            return ECommands.Default;
        }

        public override string GetCommandIconUnicode()
        {
            return "👌";
        }

        public override string GetCommandTextDescription()
        {
            return "Set default mode";
        }
    }
}
