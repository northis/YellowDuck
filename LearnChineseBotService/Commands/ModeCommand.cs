using System;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class ModeCommand : HelpCommand
    {
        public ModeCommand(Func<CommandBase[]> getAllComands):base(getAllComands)
        {
        }

        public override AnswerItem Reply(MessageItem mItem)
        {
            if (string.IsNullOrWhiteSpace(mItem.TextOnly))
            {
                return new AnswerItem
                {
                    Message = "Выберите режим:",
                    Markup = new InlineKeyboardMarkup
                    {
                        InlineKeyboard =
                            new[]
                            {
                                new[]
                                {
                                    new InlineKeyboardButton("‍🎓Обучение", "learn"),
                                    new InlineKeyboardButton("📚Словарь", "dic"),
                                    new InlineKeyboardButton("❓Команды", "help")
                                }
                            }
                    }
                };
            }

            if (mItem.TextOnly == "learn")
            {
                return new AnswerItem
                {
                    Message = "Режим обучения задан",
                    Markup = new ReplyKeyboardMarkup { Keyboard = GetLearnButtons(), ResizeKeyboard = true, OneTimeKeyboard = false }
                };
            }

            if (mItem.TextOnly == "help")
            {
                return new AnswerItem
                {
                    Message = $"Список команд{Environment.NewLine}{GetHelpMessage()}",
                    Markup =
                        new ReplyKeyboardMarkup
                        {
                            Keyboard = GetLearnButtons(),
                            ResizeKeyboard = true,
                            OneTimeKeyboard = false
                        }
                };
            }

            if (mItem.TextOnly == "dic")
            {
                return new AnswerItem
                {
                    Message = "Режим словаря задан",
                    Markup =
                        new ReplyKeyboardMarkup
                        {
                            Keyboard = GetDictionaryButtons(),
                            ResizeKeyboard = true,
                            OneTimeKeyboard = false
                        }
                };
            }

            return new AnswerItem
            {
                Message = "Задан неверный параметр"
            };
        }

        public override ECommands GetCommandType()
        {
            return ECommands.Mode;
        }

        public override string GetCommandDescription()
        {
            return "⚙️Открыть меню выбора режима";
        }
    }
}
