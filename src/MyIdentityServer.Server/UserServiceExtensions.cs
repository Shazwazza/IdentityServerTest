using System.Data.Entity;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using MyIdentityServer.Core;
using MyIdentityServer.Core.Models;
using MyIdentityServer.Server.Services;

namespace MyIdentityServer.Server
{
    public static class UserServiceExtensions
    {
        public static void ConfigureUserService(this IdentityServerServiceFactory factory)
        {
            factory.UserService = new Registration<IUserService, UserService>();
            factory.Register(new Registration<ApplicationUserManager>(
                resolver => ApplicationUserManager.CreateUserManager(
                    new IdentityFactoryOptions<ApplicationUserManager>(),
                    resolver.Resolve<DbContext>(),
                    new EmailService(),
                    new SmsService())));
            factory.Register(new Registration<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>());
            factory.Register(new Registration<DbContext>(resolver => new ApplicationDbContext()));
        }
    }
}