using System.IO;
using Telegram.Bot.Types;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public abstract class CommandBase
    {
        public abstract AnswerItem Reply(MessageItem mItem);
        public abstract ECommands GetCommandType();


        public KeyboardButton[] GetDefaultButtons()
        {
            return new[]
            {
                 new KeyboardButton{Text = $"/{ECommands.Import} Импорт слов"}
            };
        }
    }
}
