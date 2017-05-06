using System.Threading.Tasks;
using System.Web.Http;
using Telegram.Bot.Types;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace YellowDuck.LearnChineseBotService.WebHook
{
    public class WebHookController : ApiController
    {
        private readonly QueryHandler _queryHandler;

        public WebHookController( QueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }
        
        public async Task<IHttpActionResult> Post(Update update)
        {
            if(update.Message !=null)
                await _queryHandler.OnMessage(update.Message);

            if (update.CallbackQuery != null)
                await _queryHandler.CallbackQuery(update.CallbackQuery);

            if (update.InlineQuery != null)
                await _queryHandler.InlineQuery(update.InlineQuery);
            

            return Ok();
        }
    }
}