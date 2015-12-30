using System.Data.Entity;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using MyIdentityServer.Core;
using MyIdentityServer.Core.Models;

namespace MyIdentityServer.Server.Services
{
    public static class UserServiceExtensions
    {
        public static void ConfigureUserService(this IdentityServerServiceFactory factory)
        {
            factory.UserService = new IdentityServer3.Core.Configuration.Registration<IUserService, ApplicationUserService>();
            factory.Register(new IdentityServer3.Core.Configuration.Registration<ApplicationUserManager>(
                resolver => ApplicationUserManager.CreateUserManager(
                    new IdentityFactoryOptions<ApplicationUserManager>(),
                    resolver.Resolve<DbContext>(),
                    new EmailService(),
                    new SmsService())));
            factory.Register(new IdentityServer3.Core.Configuration.Registration<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>());
            factory.Register(new IdentityServer3.Core.Configuration.Registration<DbContext>(resolver => new ApplicationDbContext()));
        }
    }
}