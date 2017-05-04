using YellowDuck.LearnChineseBotService.Commands.Common;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class DefaultCommand: CommandBase
    {
        public override AnswerItem Reply(MessageItem mItem)
        {
            return new AnswerItem
            {
                Message = "Defaut mode has been set"
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
