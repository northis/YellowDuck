using System;
using System.Diagnostics;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChineseBotService.Commands.Common;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class AddCommand : ImportCommand
    {
        public AddCommand(IChineseWordParseProvider parseProvider, IWordRepository repository,
            IFlashCardGenerator flashCardGenerator) : base(parseProvider, repository, flashCardGenerator)
        {
        }

        public override AnswerItem Reply(MessageItem mItem)
        {
            var addMessage =
                $"Type a chinese word in '<word>{SeparatorChar}<translation>' format";

            if (string.IsNullOrEmpty(mItem.TextOnly))
            {
                return new AnswerItem { Message = addMessage };
            }

            try
            {
                var result = SaveAnswerItem(new[] {mItem.TextOnly}, mItem.UserId);

                if (result == null)
                    return new AnswerItem
                    {
                        Message = $"Bad string format.{Environment.NewLine}{addMessage}"
                    };

                return result;

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return new AnswerItem { Message = $"Wrong format.{Environment.NewLine}{addMessage}" };
            }
        }

        public override ECommands GetCommandType()
        {
            return ECommands.Add;
        }



        public override string GetCommandIconUnicode()
        {
            return "➕";
        }

        public override string GetCommandTextDescription()
        {
            return "Add a new chinese word";
        }
    }
}
