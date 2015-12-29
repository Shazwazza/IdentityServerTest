﻿using System;
using System.Collections.Generic;
using IdentityServer3.Core.Configuration;
using IdentityServer3.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using MyIdentityServer.Server;
using MyIdentityServer.Server.Services;
using System.Linq;
using IdentityManager.Configuration;
using IdentityServer3.Core.Models;
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

            // these two calls just pre-populate the test DB from the in-memory config
            ConfigureClients(Clients.Get(), efConfig);
            ConfigureScopes(Scopes.Get(), efConfig);

            app.Map("/admin", adminApp =>
            {
                var imgrFactory = new IdentityManagerServiceFactory();
                imgrFactory.ConfigureSimpleIdentityManagerService();
                //factory.ConfigureCustomIdentityManagerServiceWithIntKeys("AspId_CustomPK");

                adminApp.UseIdentityManager(new IdentityManagerOptions()
                {
                    Factory = imgrFactory
                });
            });

            app.Map("/id", adminApp =>
            {
                var factory = new IdentityServerServiceFactory();

                factory.RegisterConfigurationServices(efConfig);
                factory.RegisterOperationalServices(efConfig);

                //factory.UseInMemoryUsers(Users.Get());            
                //TODO: enable EF users
                factory.ConfigureUserService();

                var options = new IdentityServerOptions
                {
                    SiteName = "MyIdentityServer - Server",
                    Factory = factory,
                    RequireSsl = false,
                    SigningCertificate = Certificate.Get(),
                };

                app.UseIdentityServer(options);
            });

            var cleanup = new TokenCleanup(efConfig, 10);
            cleanup.Start();
        }

        

        public static void ConfigureClients(IEnumerable<Client> clients, EntityFrameworkServiceOptions options)
        {
            using (var db = new ClientConfigurationDbContext(options.ConnectionString, options.Schema))
            {
                if (!db.Clients.Any())
                {
                    foreach (var c in clients)
                    {
                        var e = c.ToEntity();
                        db.Clients.Add(e);
                    }
                    db.SaveChanges();
                }
            }
        }

        public static void ConfigureScopes(IEnumerable<Scope> scopes, EntityFrameworkServiceOptions options)
        {
            using (var db = new ScopeConfigurationDbContext(options.ConnectionString, options.Schema))
            {
                if (!db.Scopes.Any())
                {
                    foreach (var s in scopes)
                    {
                        var e = s.ToEntity();
                        db.Scopes.Add(e);
                    }
                    db.SaveChanges();
                }
            }
        }
    }
}