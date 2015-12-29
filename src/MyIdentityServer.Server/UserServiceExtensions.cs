using System.Data.Entity;
using IdentityManager;
using IdentityManager.Configuration;
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
            factory.UserService = new IdentityServer3.Core.Configuration.Registration<IUserService, UserService>();
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

    public static class SimpleIdentityManagerServiceExtensions
    {
        public static void ConfigureSimpleIdentityManagerService(this IdentityManagerServiceFactory factory)
        {
            factory.Register(new IdentityManager.Configuration.Registration<DbContext>(resolver => new ApplicationDbContext()));
            factory.Register(new IdentityManager.Configuration.Registration<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>());
            factory.Register(new IdentityManager.Configuration.Registration<ApplicationUserManager>(
                resolver => ApplicationUserManager.CreateUserManager(
                    new IdentityFactoryOptions<ApplicationUserManager>(),
                    resolver.Resolve<DbContext>(),
                    new EmailService(),
                    new SmsService())));
            factory.Register(new IdentityManager.Configuration.Registration<IRoleStore<ApplicationRole, string>, RoleStore<ApplicationRole>>());
            factory.Register(new IdentityManager.Configuration.Registration<ApplicationRoleManager>());
            factory.IdentityManagerService = new IdentityManager.Configuration.Registration<IIdentityManagerService, SimpleIdentityManagerService>();
        }
    }
}