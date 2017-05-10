using YellowDuck.LearnChinese.Data.ObjectModels;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChineseBotService.Commands.Common;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class LearnSpeakCommand : LearnCommand
    {
        private readonly IStudyProvider _studyProvider;

        public LearnSpeakCommand(IStudyProvider studyProvider, EditCommand editCommand) : base(studyProvider, editCommand)
        {
            _studyProvider = studyProvider;
        }

        public override ECommands GetCommandType()
        {
            return ECommands.LearnPronunciation;
        }

        public override string GetCommandIconUnicode()
        {
            return "📢";
        }

        public override string GetCommandTextDescription()
        {
            return "Learn how to prononciate these words";
        }

        public override LearnUnit ProcessLearn(MessageItem mItem)
        {
            return _studyProvider.LearnWord(mItem.ChatId, ELearnMode.Pronunciation);
        }
        
    }
}
