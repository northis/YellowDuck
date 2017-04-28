using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Owin;
using YellowDuck.LearnChineseBotService.LayoutRoot;

namespace YellowDuck.LearnChineseBotService.WebHook
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var configuration = new HttpConfiguration();

            configuration.Routes.MapHttpRoute("DefaultApi","{controller}/{id}",
      new { id = RouteParameter.Optional }
 );

            //MapHttpRoute("WebHook", "{controller}");
            
            app.UseWebApi(configuration);
            
        }
    }
}