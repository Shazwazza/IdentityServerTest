using IdentityManager.AspNetIdentity;
using MyIdentityServer.Core;
using MyIdentityServer.Core.Models;

namespace MyIdentityServer.Server
{
    public class SimpleIdentityManagerService : AspNetIdentityManagerService<ApplicationUser, string, ApplicationRole, string>
    {
        public SimpleIdentityManagerService(ApplicationUserManager userMgr, ApplicationRoleManager roleMgr)
            : base(userMgr, roleMgr)
        {
        }
    }
}