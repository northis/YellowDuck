using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChineseBotService.LayoutRoot;
using User = YellowDuck.LearnChinese.Data.Ef.User;

namespace YellowDuck.LearnChineseBotService.MainExecution
{
    public class QueryHandler
    {
        private readonly TelegramBotClient _client;
        private readonly IWordRepository _repository;

        public QueryHandler(TelegramBotClient client, IWordRepository repository)
        {
            _client = client;
            _repository = repository;
            
        }

        public async Task InlineQuery(InlineQuery inlineQuery)
        {

        }

        public async Task InlineResultChosen(ChosenInlineResult chosenInlineResult)
        {

        }

        public async Task CallbackQuery(CallbackQuery callbackQuery)
        {
            try
            {
                await OnMessage(callbackQuery.Message, callbackQuery.Data, callbackQuery.From);
            }
            catch (Exception ex)
            {
                MainFactory.Log.Write("CallbackQuery", ex, null);
            }
        }

        public async Task OnMessage(Message msg)
        {
            try
            {
                await OnMessage(msg, msg.Text, msg.From);
            }
            catch (Exception ex)
            {
                MainFactory.Log.Write("Message", ex, null);
            }
        }

        public void OnReceiveGeneralError(Exception e)
        {
            MainFactory.Log.Write(nameof(OnReceiveGeneralError), e, null);
        }

        public void OnReceiveError(ApiRequestException e)
        {
            MainFactory.Log.Write(nameof(OnReceiveError), e, null);
        }

        async Task OnMessage(Message msg, string argumentCommand, Telegram.Bot.Types.User user)
        {
            if (!_repository.IsUserExist(user.Id))
                _repository.AddUser(new User
                {
                    IdUser = user.Id,
                    Name = $"{user.FirstName} {user.LastName}"
                });

            var firstEntity = msg.Entities.FirstOrDefault();
            if (firstEntity?.Type == MessageEntityType.BotCommand)
            {
                var commandOnly = msg.Text.Substring(firstEntity.Offset, firstEntity.Length);

                var noEmoji = GetNoEmojiString(msg.Text);

                var textOnly = noEmoji.Replace(commandOnly,string.Empty);
                await HandleCommand(new MessageItem
                {
                    Command = commandOnly,
                    ChatId = msg.Chat.Id,
                    UserId = user.Id,
                    Text = msg.Text,
                    TextOnly = textOnly
                });

                _repository.SetUserCommand(user.Id, commandOnly);
            }
            else
            {
                await HandleArgumentCommand(msg, argumentCommand, user.Id);
            }
        }

        async Task HandleArgumentCommand(Message msg, string argumentCommand, long userId)
        {
            var possiblePreviousCommand = _repository.GetUserCommand(userId);

            if (!string.IsNullOrWhiteSpace(possiblePreviousCommand))
            {
                var noEmojiCmd = GetNoEmojiString(argumentCommand);

                var mItem = new MessageItem
                {
                    Command = possiblePreviousCommand,
                    ChatId = msg.Chat.Id,
                    UserId = msg.From.Id,
                    Text = msg.Text,
                    TextOnly = noEmojiCmd.Replace(possiblePreviousCommand, string.Empty)
                };

                if (msg.Document != null)
                {
                    var file = await _client.GetFileAsync(msg.Document.FileId);
                    mItem.FileStream = file.FileStream;
                }

                await HandleCommand(mItem);
            }
        }

        async Task HandleCommand(MessageItem mItem)
        { 
            var handler = MainFactory.GetCommandHandler(mItem.Command);
            var reply = handler.Reply(mItem);

            if (reply.Picture == null)
            {
                await _client.SendTextMessageAsync(mItem.ChatId, reply.Message, true, false, 0, reply.Markup);
            }
            else
            {
                using (var ms = new MemoryStream(reply.Picture))
                {
                   await _client.SendPhotoAsync(mItem.ChatId, new FileToSend(Guid.NewGuid() + ".png", ms), reply.Message, false,
                        0, reply.Markup);
                }

            }
        }

        public static string GetNoEmojiString(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            var noEmojiStr = new StringBuilder();

            foreach (var chr in str)
            {
                var cat = char.GetUnicodeCategory(chr);

                if(cat != UnicodeCategory.OtherSymbol && cat != UnicodeCategory.Surrogate && cat != UnicodeCategory.NonSpacingMark)
                    noEmojiStr.Append(chr);
            }

            return noEmojiStr.ToString();
        }
    }

}
