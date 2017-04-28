using System;
using Microsoft.Owin.Hosting;
using Telegram.Bot;

namespace YellowDuck.LearnChineseBotService.WebHook
{
    public class WebServer
    {
        private readonly TelegramBotClient _client;
        private readonly string _innerurl;
        private readonly string _outerUrl;

        public IDisposable WebHookWebServer { get; private set; }

        public WebServer(string webhookUrl, string webhookPublicUrl, TelegramBotClient client)
        {
            _innerurl = webhookUrl;
            _outerUrl = webhookPublicUrl;
            _client = client;
        }

        public void Start()
        {
            if (WebHookWebServer != null)
                return;
            
            WebHookWebServer = WebApp.Start<Startup>(_innerurl);
            _client.SetWebhook(_outerUrl).Wait();
        }

        public void Stop()
        {
            _client.SetWebhook().Wait();
            WebHookWebServer.Dispose();
        }
    }
}