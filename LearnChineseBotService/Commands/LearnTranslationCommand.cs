using YellowDuck.LearnChinese.Data.ObjectModels;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChineseBotService.Commands.Common;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class LearnTranslationCommand : LearnCommand
    {
        private readonly IStudyProvider _studyProvider;

        public LearnTranslationCommand(IStudyProvider studyProvider, EditCommand editCommand) : base(studyProvider,
            editCommand)
        {
            _studyProvider = studyProvider;
        }

        public override string GetCommandIconUnicode()
        {
            return "🇨🇳";
        }

        public override string GetCommandTextDescription()
        {
            return "Learn what these words mean";
        }

        public override ECommands GetCommandType()
        {
            return ECommands.LearnTranslation;
        }

        public override LearnUnit ProcessLearn(MessageItem mItem)
        {
            return _studyProvider.LearnWord(mItem.ChatId, ELearnMode.Translation);
        }
    }
}