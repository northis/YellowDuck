using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Ninject;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using YellowDuck.LearnChineseBotService.LayoutRoot;

namespace YellowDuck.LearnChineseBotService.WebHook
{
    public class WebHookController : ApiController
    {
        private readonly TelegramBotClient _client;

        public WebHookController()
        {
            _client = MainFactory.NinjectKernel.Get<TelegramBotClient>();
        }

        public IHttpActionResult Get()
        {
            return Ok();
        }
        
        public async Task<IHttpActionResult> Post(Update update)
        {
            var message = update.Message;

            Console.WriteLine("Received Message from {0}", message.Chat.Id);

            if (message.Type == MessageType.TextMessage)
            {
                // Echo each Message
                await _client.SendTextMessage(message.Chat.Id, message.Text);
            }

            return Ok();
        }
    }
}