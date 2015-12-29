using Microsoft.Owin;
using MyIdentityServer.Website;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace MyIdentityServer.Website
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
