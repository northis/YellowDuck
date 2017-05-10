using System;
using System.Diagnostics;
using YellowDuck.LearnChineseBotService.Commands.Common;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class EditCommand : CommandBase
    {
        public EditCommand() : base()
        {
        }

        public override AnswerItem Reply(MessageItem mItem)
        {
            try
            {
                return new AnswerItem
                {
                    Message = $".{Environment.NewLine}{1}"
                };

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return new AnswerItem { Message = $"Wrong format.{Environment.NewLine}{1}" };
            }
        }

        public override ECommands GetCommandType()
        {
            return ECommands.Edit;
        }

        public override string GetCommandIconUnicode()
        {
            return "🖌";
        }

        public override string GetCommandTextDescription()
        {
            return "Edit an existing chinese word";
        }
    }
}
