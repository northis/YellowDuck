using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChineseBotService.Commands.Common;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class ModeCommand : CommandBase
    {
        private readonly IWordRepository _repository;

        public ModeCommand(IWordRepository repository)
        {
            _repository = repository;
        }

        public override string GetCommandIconUnicode()
        {
            return "⚙️";
        }

        public override string GetCommandTextDescription()
        {
            return "Choose learn words mode";
        }

        public override ECommands GetCommandType()
        {
            return ECommands.Mode;
        }

        public override AnswerItem Reply(MessageItem mItem)
        {
            if (string.IsNullOrWhiteSpace(mItem.TextOnly))
                return new AnswerItem
                {
                    Message = "Choose learn words mode:",
                    Markup = new InlineKeyboardMarkup
                    {
                        InlineKeyboard =
                            new InlineKeyboardButton[][]
                            {
                                new[]
                                {
                                    new InlineKeyboardCallbackButton("‍Hard, old first",
                                        EGettingWordsStrategy.OldMostDifficult.ToString())
                                },
                                new[]
                                {
                                    new InlineKeyboardCallbackButton("‍Hard, new first",
                                        EGettingWordsStrategy.NewMostDifficult.ToString())
                                },
                                new[]
                                {
                                    new InlineKeyboardCallbackButton("‍New first", EGettingWordsStrategy.NewFirst.ToString())
                                },
                                new[]
                                {
                                    new InlineKeyboardCallbackButton("‍Old first", EGettingWordsStrategy.OldFirst.ToString())
                                },
                                new[]
                                {
                                    new InlineKeyboardCallbackButton("‍Random", EGettingWordsStrategy.Random.ToString())
                                }
                            }
                    }
                };

            if (Enum.TryParse(mItem.TextOnly, true, out EGettingWordsStrategy strategy))
                try
                {
                    _repository.SetLearnMode(mItem.ChatId, strategy);
                }
                catch (Exception e)
                {
                    return new AnswerItem
                    {
                        Message = e.Message
                    };
                }
            else
                return new AnswerItem
                {
                    Message = "This mode is not supported"
                };

            return new AnswerItem
            {
                Message = "Mode has been set"
            };
        }
    }
}