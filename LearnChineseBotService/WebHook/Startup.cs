using System.Web.Http;
using Owin;
using YellowDuck.LearnChineseBotService.LayoutRoot;

namespace YellowDuck.LearnChineseBotService.WebHook
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var configuration = new HttpConfiguration();

            configuration.Routes.MapHttpRoute("DefaultApi", MainFactory.TelegramBotKey + "/{controller}/{id}",
                new {id = RouteParameter.Optional}
            );
            
            app.UseWebApi(configuration);
            
        }
    }
}