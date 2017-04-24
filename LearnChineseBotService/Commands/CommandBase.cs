using System;
using Telegram.Bot.Types;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public abstract class CommandBase
    {
        public abstract AnswerItem Reply(MessageItem mItem);
        public abstract ECommands GetCommandType();

        public const string CommandStartChar = "/";

        public static ECommands GetCommandType(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command), "Команда не может быть пустой");

            if (!command.StartsWith(CommandStartChar))
                throw new ArgumentException($"Команда должна начинаться с символа '{CommandStartChar}'", nameof(command));

            var cleanedCommand = command.Substring(1, command.Length - 1);

            if (!Enum.TryParse(cleanedCommand, true, out ECommands commandEnum))
                throw new NotSupportedException($"Команда '{ nameof(command)}' не поддерживается");

            return commandEnum;
        }


        public virtual string GetCommandDescription()
        {
            return GetCommandType().ToString();
        }

        public string GetFormattedDescription()
        {
            return $"{CommandStartChar}{GetCommandType()} - {GetCommandDescription()}";
        }

        public virtual KeyboardButton[][] GetDictionaryButtons()
        {
            return new[]
            {
                new[]
                {
                    new KeyboardButton {Text = $"🚛{CommandStartChar}{ECommands.Import}"},
                    new KeyboardButton {Text = $"➕{CommandStartChar}{ECommands.Add}"},
                    new KeyboardButton {Text = $"⚙️{CommandStartChar}{ECommands.Mode}"}
                },

                new[]
                {
                    new KeyboardButton {Text = $"👀{CommandStartChar}{ECommands.View}"},
                    new KeyboardButton {Text = $"🗑{CommandStartChar}{ECommands.Delete}"},
                    new KeyboardButton {Text = $"🤝{CommandStartChar}{ECommands.Share}"},
                    new KeyboardButton {Text = $"❓{CommandStartChar}{ECommands.Help}"}
                }
            };
        }

        public virtual KeyboardButton[][] GetLearnButtons()
        {
            return new[]
            {
                new[]
                {
                    new KeyboardButton {Text = $"⚙️{CommandStartChar}{ECommands.Mode}"},
                    new KeyboardButton {Text = $"❓{CommandStartChar}{ECommands.Help}"}
                }
            };
        }
    }
}
