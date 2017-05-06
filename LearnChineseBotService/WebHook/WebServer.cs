using System;
using Microsoft.Owin.Hosting;
using Telegram.Bot;

namespace YellowDuck.LearnChineseBotService.WebHook
{            // Endpoint musst be configured with netsh:
             // netsh http add urlacl url=https://+:443/ user=<username>
             // netsh http add sslcert ipport=0.0.0.0:443 certhash=<cert thumbprint> appid=<random guid>
    public class WebServer
    {
        private readonly TelegramBotClient _client;
        private readonly string _innerurl;
        private readonly string _outerUrl;
        private readonly string _telegramId;
        private readonly string _whControllerName;

        public IDisposable WebHookWebServer { get; private set; }

        public WebServer(string webhookUrl, string webhookPublicUrl, string telegramId, string whControllerName, TelegramBotClient client)
        {
            _innerurl = webhookUrl;
            _outerUrl = webhookPublicUrl;
            _telegramId = telegramId;
            _whControllerName = whControllerName;
            _client = client;
        }

        public void Start()
        {
            if (WebHookWebServer != null)
                return;

            //NOTE This bot uses the webhooks web-server to provide a jpeg-url for the inline mode. So don't use irregular ports (such as 8433), because Telegram cannot show a jpeg picture inside an inline message while you use those ports and the message will be hold in not-delivered status forever. Use the 443 port and don't specify the port in the url.
            WebHookWebServer = WebApp.Start<Startup>(_innerurl);
            _client.SetWebhook(_outerUrl + $"/{_telegramId}/{_whControllerName}").Wait();
        }

        public void Stop()
        {
            _client.SetWebhook().Wait();
            WebHookWebServer.Dispose();
        }
    }
}