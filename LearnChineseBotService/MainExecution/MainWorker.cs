using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChineseBotService.Commands.Enums;
using YellowDuck.LearnChineseBotService.LayoutRoot;
using User = YellowDuck.LearnChinese.Data.Ef.User;

namespace YellowDuck.LearnChineseBotService.MainExecution
{

    public class MainWorker
    {
        private readonly TelegramBotClient _client;
        private readonly IWordRepository _repository;
        
        public MainWorker(TelegramBotClient client, IWordRepository repository)
        {
            _client = client;
            _repository = repository;
            _client.OnMessage += TryClientOnMessage;
            _client.OnReceiveError += _client_OnReceiveError;
            _client.OnReceiveGeneralError += _client_OnReceiveGeneralError;
            _client.OnCallbackQuery += TryOnCallbackQuery;

            
        }

        private async void TryOnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            try
            {
                await OnOnCallbackQuery(e);
            }
            catch (Exception ex)
            {
                MainFactory.Log.Write("OnCallbackQuery", ex, null);
            }
        }

        private async Task OnOnCallbackQuery(CallbackQueryEventArgs e)
        {
            await CheckUser(e.CallbackQuery.From);
            await HandleArgumentCommand(e.CallbackQuery.Message, e.CallbackQuery.Data,e.CallbackQuery.From.Id);
        }

        private async void TryClientOnMessage(object sender, MessageEventArgs e)
        {
            try
            {
                 await OnMessage(e.Message);
            }
            catch (Exception ex)
            {
                MainFactory.Log.Write("OnMessage", ex, null);
            }
        }

        async Task CheckUser(Telegram.Bot.Types.User user)
        {
            if (!_repository.IsUserExist(user.Id))
                _repository.AddUser(new User
                {
                    IdUser = user.Id,
                    Name = $"{user.FirstName} {user.LastName} ({user.Username})"
                });
        }

        async Task OnMessage(Message msg)
        {
            var userId = msg.From.Id;

            await CheckUser(msg.From);

            var firstEntity = msg.Entities.FirstOrDefault();
            if (firstEntity?.Type == MessageEntityType.BotCommand)
            {
                var commandOnly = msg.Text.Substring(firstEntity.Offset, firstEntity.Length);

                var noEmoji = GetNoEmojiString(msg.Text);

                var textOnly = noEmoji.Replace(commandOnly,"");
                await HandleCommand(new MessageItem
                {
                    Command = commandOnly,
                    ChatId = msg.Chat.Id,
                    UserId = userId,
                    Text = msg.Text,
                    TextOnly = textOnly
                });

                _repository.SetUserCommand(userId, commandOnly);
            }
            else
            {
                await HandleArgumentCommand(msg, msg.Text,userId);
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

        private void _client_OnReceiveGeneralError(object sender, ReceiveGeneralErrorEventArgs e)
        {
            MainFactory.Log.Write(nameof(_client_OnReceiveGeneralError), e.Exception, null);
        }

        private void _client_OnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
            MainFactory.Log.Write(nameof(_client_OnReceiveError), e.ApiRequestException, null);
        }

        public void Start()
        {
            _client.StartReceiving();
        }

        public void Stop()
        {
            _client.StopReceiving();
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
