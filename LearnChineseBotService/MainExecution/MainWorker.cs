using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using YellowDuck.LearnChinese.Interfaces;
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
                MainFactory.Log.Write("OnOnCallbackQuery", ex, null);
            }
        }

        private async Task OnOnCallbackQuery(CallbackQueryEventArgs e)
        {
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

        private async Task OnMessage(Message msg)
        {
            var userId = msg.From.Id;

            if (msg.NewChatMember != null)
                _repository.AddUser(new User
                {
                    IdUser = userId,
                    Name = msg.NewChatMember.Username
                });

            var firstEntity = msg.Entities.FirstOrDefault();

            if (firstEntity?.Type == MessageEntityType.BotCommand)
            {
                var commandOnly = msg.Text.Substring(firstEntity.Offset, firstEntity.Length);
                var textOnly = msg.Text.Replace(commandOnly,"");
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
                var mItem = new MessageItem
                {
                    Command = possiblePreviousCommand,
                    ChatId = msg.Chat.Id,
                    UserId = msg.From.Id,
                    Text = msg.Text,
                    TextOnly = argumentCommand.Replace(possiblePreviousCommand, string.Empty)
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
    }
}
