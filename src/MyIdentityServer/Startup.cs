using Microsoft.Owin;
using MyIdentityServer;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace MyIdentityServer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
