using System;
using System.Linq;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class HelpCommand : CommandBase
    {
        protected Func<CommandBase[]> GetAllComands { get; }

        public HelpCommand(Func<CommandBase[]> getAllComands)
        {
            GetAllComands = getAllComands;
        }

        public override AnswerItem Reply(MessageItem mItem)
        {
            return new AnswerItem
            {
                Message = GetHelpMessage()
            };
        }

        public override ECommands GetCommandType()
        {
            return ECommands.Help;
        }

        public override string GetCommandDescription()
        {
            return "❓Список команд";
        }

        public virtual string GetHelpMessage()
        {
            return string.Join(Environment.NewLine, GetAllComands().Select(a => a.GetFormattedDescription()));
        }
    }
}
