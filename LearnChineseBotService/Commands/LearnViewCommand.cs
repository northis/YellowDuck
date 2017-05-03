using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class LearnViewCommand : CommandBase
    {
        private readonly IStudyProvider _studyProvider;

        public LearnViewCommand(IStudyProvider studyProvider)
        {
            _studyProvider = studyProvider;
        }

        public override AnswerItem Reply(MessageItem mItem)
        {
            var newWord = _studyProvider.LearnWord(mItem.ChatId, ELearnMode.FullView,
                EGettingWordsStrategy.OldMostDifficult);
            //TODO Вывести еще статистику сюда
            return new AnswerItem
            {
                Message = GetCommandIconUnicode(),
                Picture = newWord.Picture,
                Markup = GetLearnMarkup()
            };
        }


        public override ECommands GetCommandType()
        {
            return ECommands.LearnView;
        }

        public override string GetCommandIconUnicode()
        {
            return "🎓👀";
        }

        public override string GetCommandTextDescription()
        {
            return "Just view some words";
        }
    }
}
