using System.Web.Http;
using System.Web.Http.Routing;
using Ninject.WebApi.DependencyResolver;
using Owin;
using YellowDuck.LearnChineseBotService.LayoutRoot;

namespace YellowDuck.LearnChineseBotService.WebHook
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var configuration = new HttpConfiguration();

            //configuration.Routes.MapHttpRoute("DefaultApi", MainFactory.TelegramBotKey + "/{controller}/{id}",
            //    new {id = RouteParameter.Optional}
            //);

            configuration.Routes.Add("DefaultApi", new HttpRoute(MainFactory.TelegramBotKey + "/{controller}"));
            configuration.Routes.Add("FileApi",
                new HttpRoute("{controller}/{fileId}",
                    new HttpRouteValueDictionary {{"fileId", RouteParameter.Optional}},
                    new HttpRouteValueDictionary {{"controller", "FlashCard"}}));

            configuration.DependencyResolver = new NinjectDependencyResolver(MainFactory.NinjectKernel);
            app.UseWebApi(configuration);
            
        }
    }
}