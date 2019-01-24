using Owin;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace CdsaContentManager {
  public partial class Startup {

    public void Configuration(IAppBuilder app) {
      ConfigureAuth(app);
      AreaRegistration.RegisterAllAreas();
      GlobalConfiguration.Configure(WebApiConfig.Register);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
    }

  }
}