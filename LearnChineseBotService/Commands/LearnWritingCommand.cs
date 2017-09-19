using YellowDuck.LearnChinese.Data.ObjectModels;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChineseBotService.Commands.Common;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class LearnWritingCommand : LearnCommand
    {
        private readonly IStudyProvider _studyProvider;

        public LearnWritingCommand(IStudyProvider studyProvider, EditCommand editCommand) : base(studyProvider,
            editCommand)
        {
            _studyProvider = studyProvider;
        }

        public override string GetCommandIconUnicode()
        {
            return "🖌";
        }

        public override string GetCommandTextDescription()
        {
            return "Learn how to write these words";
        }

        public override ECommands GetCommandType()
        {
            return ECommands.LearnWriting;
        }

        public override LearnUnit ProcessLearn(MessageItem mItem)
        {
            return _studyProvider.LearnWord(mItem.ChatId, ELearnMode.OriginalWord);
        }
    }
}