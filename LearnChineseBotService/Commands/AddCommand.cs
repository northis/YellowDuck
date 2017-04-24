using System;
using System.Diagnostics;
using YellowDuck.LearnChinese.Interfaces;
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
                $"Напишите слово в формате <слово/фраза иероглифами>{SeparatorChar}<перевод>";

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
                        Message = $"Строка плохого формата.{Environment.NewLine}{addMessage}"
                    };

                return result;

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return new AnswerItem { Message = $"Формат неверный.{Environment.NewLine}{addMessage}" };
            }
        }

        public override ECommands GetCommandType()
        {
            return ECommands.Add;
        }

        public override string GetCommandDescription()
        {
            return "➕Добавить новое слово/фразу";
        }
    }
}
