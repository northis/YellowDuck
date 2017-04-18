using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using YellowDuck.LearnChinese.Data.Ef;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChineseBotService.Commands;
using YellowDuck.LearnChineseBotService.LayoutRoot;

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

        private void _client_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.NewChatMember != null)
                _repository.AddUser(new User
                {
                    IdUser = e.Message.NewChatMember.Id,
                    Name = e.Message.NewChatMember.Username
                });

            var firstEntity = e.Message.Entities.FirstOrDefault();

            if (firstEntity?.Type == MessageEntityType.BotCommand)
            {
                var commandOnly = e.Message.Text.Substring(firstEntity.Offset, firstEntity.Length);

                var handler = MainFactory.GetCommandHandler(commandOnly);
                var reply = handler.Reply(e.Message);

                _client.SendTextMessageAsync(e.Message.Chat.Id, reply.Message, true, false, 0, reply.Markup);

                _repository.SetUserCommand(e.Message.From.Id, commandOnly);
            }
            else
            {

            }

        }
    }
}
