using Telegram.Bot;
using Telegram.Bot.Args;

namespace YellowDuck.LearnChineseBotService.MainExecution
{

    public class PollWorker
    {
        private readonly TelegramBotClient _client;
        private readonly QueryHandler _queryHandler;

        public PollWorker(TelegramBotClient client, QueryHandler queryHandler)
        {
            _client = client;
            _queryHandler = queryHandler;

            _client.OnMessage += _client_OnMessage;
            _client.OnReceiveError += _client_OnReceiveError;
            _client.OnReceiveGeneralError += _client_OnReceiveGeneralError;
            _client.OnCallbackQuery += _client_OnCallbackQuery;
            _client.OnInlineQuery += _client_OnInlineQuery;


        }

        private void _client_OnInlineQuery(object sender, InlineQueryEventArgs e)
        {
            _queryHandler.InlineQuery(e.InlineQuery);
        }

        private void _client_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            _queryHandler.CallbackQuery(e.CallbackQuery);
        }

        private void _client_OnReceiveGeneralError(object sender, ReceiveGeneralErrorEventArgs e)
        {
            _queryHandler.OnReceiveGeneralError(e.Exception);
        }

        private void _client_OnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
            _queryHandler.OnReceiveError(e.ApiRequestException);
        }

        private void _client_OnMessage(object sender, MessageEventArgs e)
        {
            _queryHandler.OnMessage(e.Message);
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
