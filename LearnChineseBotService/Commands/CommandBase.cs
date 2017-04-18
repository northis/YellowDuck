using System;
using Telegram.Bot.Types;
using YellowDuck.LearnChineseBotService.Commands.Enums;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public abstract class CommandBase
    {
        public abstract AnswerItem Reply(Message msg);
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
