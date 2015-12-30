using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using IdentityServer3.Core;

namespace MyIdentityServer.Website
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {   
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
