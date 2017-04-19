using System;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChineseBotService.LayoutRoot;
using User = YellowDuck.LearnChinese.Data.Ef.User;

namespace YellowDuck.LearnChineseBotService.MainExecution
{

    public class MainWorker
    {
        private readonly TelegramBotClient _client;
        private readonly IStudyProvider _studyProvider;
        private readonly IWordRepository _repository;

        public MainWorker(TelegramBotClient client, IStudyProvider studyProvider, IWordRepository repository)
        {
            _client = client;
            _studyProvider = studyProvider;
            _repository = repository;
            _client.OnMessage += _client_OnMessage;
            _client.OnMessageEdited += _client_OnMessageEdited;
            _client.OnReceiveError += _client_OnReceiveError;
            _client.OnReceiveGeneralError += _client_OnReceiveGeneralError;

            _client.OnCallbackQuery += _client_OnCallbackQuery;
            _client.OnInlineQuery += _client_OnInlineQuery;
            _client.OnInlineResultChosen += _client_OnInlineResultChosen;
            _client.OnUpdate += _client_OnUpdate;
            
        }

        public void Start()
        {
            _client.StartReceiving();
        }

        public void Stop()
        {
            _client.StopReceiving();
        }

        private void _client_OnUpdate(object sender, UpdateEventArgs e)
        {
        }

        private void _client_OnInlineResultChosen(object sender, ChosenInlineResultEventArgs e)
        {
        }

        private void _client_OnInlineQuery(object sender, InlineQueryEventArgs e)
        {
            
        }

        private void _client_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            
        }

        private void _client_OnReceiveGeneralError(object sender, ReceiveGeneralErrorEventArgs e)
        {
            MainFactory.Log.Write(nameof(_client_OnReceiveGeneralError), e.Exception, null);
        }

        private void _client_OnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
            MainFactory.Log.Write(nameof(_client_OnReceiveError), e.ApiRequestException, null);
        }

        private void _client_OnMessageEdited(object sender, MessageEventArgs e)
        {
            
        }

        private async void _client_OnMessage(object sender, MessageEventArgs e)
        {
            var userId = e.Message.From.Id;

            if (e.Message.NewChatMember != null)
                _repository.AddUser(new User
                {
                    IdUser = userId,
                    Name = e.Message.NewChatMember.Username
                });

            var firstEntity = e.Message.Entities.FirstOrDefault();

            if (firstEntity?.Type == MessageEntityType.BotCommand)
            {
                var commandOnly = e.Message.Text.Substring(firstEntity.Offset, firstEntity.Length);
                var textOnly = e.Message.Text.Replace(commandOnly,"");
                HandleCommand(new MessageItem
                {
                    Command = commandOnly,
                    ChatId = e.Message.Chat.Id,
                    UserId = userId,
                    Text = e.Message.Text,
                    TextOnly = textOnly
                });

                _repository.SetUserCommand(userId, commandOnly);
            }
            else
            {
                var possiblePreviousCommand = _repository.GetUserCommand(userId);
                if (!string.IsNullOrWhiteSpace(possiblePreviousCommand))
                {
                    var mItem = new MessageItem
                    {
                        Command = possiblePreviousCommand,
                        ChatId = e.Message.Chat.Id,
                        UserId = userId,
                        Text = e.Message.Text,
                        TextOnly = e.Message.Text.Replace(possiblePreviousCommand, "")
                    };

                    if (e.Message.Document != null)
                    {
                        var file = await _client.GetFileAsync(e.Message.Document.FileId);

                        mItem.FileStream = file.FileStream;
                    }

                    HandleCommand(mItem);
                }
            }
        }

        void HandleCommand(MessageItem mItem)
        { 
            var handler = MainFactory.GetCommandHandler(mItem.Command);
            var reply = handler.Reply(mItem);
            
            _client.SendTextMessageAsync(mItem.ChatId, reply.Message, true, false, 0, reply.Markup);
        }
    }
}
