using System.Data.Entity;
using IdentityManager;
using IdentityManager.Configuration;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using MyIdentityServer.Core;
using MyIdentityServer.Core.Models;
using MyIdentityServer.Server.Services;

namespace MyIdentityServer.Server
{
    public static class IdentityManagerServiceExtensions
    {
        public static void ConfigureSimpleIdentityManagerService(this IdentityManagerServiceFactory factory)
        {
            factory.Register(new Registration<DbContext>(resolver => new ApplicationDbContext()));
            factory.Register(new Registration<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>());
            factory.Register(new Registration<ApplicationUserManager>(
                resolver => ApplicationUserManager.CreateUserManager(
                    new IdentityFactoryOptions<ApplicationUserManager>(),
                    resolver.Resolve<DbContext>(),
                    new EmailService(),
                    new SmsService())));
            factory.Register(new Registration<IRoleStore<ApplicationRole, string>, RoleStore<ApplicationRole>>());
            factory.Register(new Registration<ApplicationRoleManager>());

            factory.IdentityManagerService = new Registration<IIdentityManagerService, ApplicationIdentityManagerService>();
        }
    }
}