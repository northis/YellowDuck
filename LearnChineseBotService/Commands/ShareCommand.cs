using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.Commands
{
    public class ShareCommand : CommandBase
    {
        public const int MaxShareUsers = 10;

        private readonly IWordRepository _repository;

        public ShareCommand(IWordRepository repository)
        {
            _repository = repository;
        }

        public override AnswerItem Reply(MessageItem mItem)
        {
            int friendUser;
            var userId = mItem.ChatId;

            var isAdd = mItem.TextOnly.StartsWith("add=");
            var isRemove = mItem.TextOnly.StartsWith("remove=");

            var parameterArray = mItem.TextOnly.Split('=');

            if (parameterArray.Length == 2 && int.TryParse(parameterArray[1], out friendUser))
            {
                if (isAdd == isRemove)
                {
                    return new AnswerItem
                    {
                        Message = "Команда не поддерживается."
                    };
                }

                try
                {
                    if (isAdd)
                    {
                        _repository.AddFriendUser(userId, friendUser);
                        return new AnswerItem
                        {
                            Message = "Список успешно расшарен другу."
                        };
                    }

                    _repository.RemoveFriendUser(userId, friendUser);
                    return new AnswerItem
                    {
                        Message = "Друг удален из списка видящих."
                    };

                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                    return new AnswerItem { Message = ex.Message };
                }
            }

            var otherUsers = _repository.GetUsers()/*.Where(a => a.IdUser != userId) TODO убрать это потом*/ ;
            var friends = _repository.GetUserFriends(userId).Select(a => a.IdUser).ToList();

            var offerFriends = otherUsers.Take(MaxShareUsers).ToArray();

            var buttons = new List<InlineKeyboardButton[]>();
            foreach (var friend in offerFriends)
            {
                var isFriendInShares = friends.Contains(friend.IdUser);

                buttons.Add(new[]
                {
                    new InlineKeyboardButton("(" + (isFriendInShares ? "➖" : "➕") + $") {friend.Name}",
                        isFriendInShares ? $"remove={friend.IdUser}" : $"add={friend.IdUser}")
                });

            }

            return new AnswerItem
            {
                Message = "Список пользователей бота. ➕ - пригласить, ➖ - отозвать приглашение",
                Markup = new InlineKeyboardMarkup
                {
                    InlineKeyboard = buttons.ToArray()
                }
            };
        }

        public override ECommands GetCommandType()
        {
            return ECommands.Share;
        }

        public override string GetCommandDescription()
        {
            return "🤝Поделиться своим списком слов с другом";
        }
    }
}
