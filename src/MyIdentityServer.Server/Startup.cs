using System;
using System.Collections.Generic;
using System.Data.Entity;
using IdentityServer3.Core.Configuration;
using IdentityServer3.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using MyIdentityServer.Server;
using MyIdentityServer.Server.Services;
using System.Linq;
using IdentityManager.Configuration;
using IdentityServer3.Core.Models;
using MyIdentityServer.Core;
using MyIdentityServer.Core.Models;
using MyIdentityServer.Server.Temp;
using Owin;
using Serilog;

[assembly: OwinStartup(typeof(Startup))]
namespace MyIdentityServer.Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            //Log.Logger = new LoggerConfiguration()
            //    .WriteTo.RollingFile("log-{Date}.txt")
            //    .CreateLogger();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Trace()
                .CreateLogger();
            
            var efConfig = new EntityFrameworkServiceOptions
            {
                ConnectionString = "DefaultConnection"
            };

#if DEBUG
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ApplicationDbContext>());
#endif

            app.Map("/admin", adminApp =>
            {
                var imgrFactory = new IdentityManagerServiceFactory();
                imgrFactory.ConfigureSimpleIdentityManagerService();

                adminApp.UseIdentityManager(new IdentityManagerOptions()
                {
                    Factory = imgrFactory                    
                });
            });

            var factory = new IdentityServerServiceFactory();

            factory.RegisterConfigurationServices(efConfig);
            factory.RegisterOperationalServices(efConfig);            
            factory.ConfigureUserService();

#if DEBUG
            // these two calls just pre-populate the test DB from the in-memory config
            TestClients.ConfigureClients(TestClients.Get(), efConfig);
            TestScopes.ConfigureScopes(TestScopes.Get(), efConfig);
#endif

            var options = new IdentityServerOptions
            {
                SiteName = "MyIdentityServer - Server",
                Factory = factory,
                RequireSsl = false,
                SigningCertificate = Certificate.Get(),
            };
            app.UseIdentityServer(options);
            
            var cleanup = new TokenCleanup(efConfig, 10);
            cleanup.Start();
        }
    }
}
