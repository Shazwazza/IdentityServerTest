using System;
using System.Linq;
using IdentityManager;
using IdentityManager.AspNetIdentity;
using Microsoft.AspNet.Identity.EntityFramework;
using MyIdentityServer.Core;
using MyIdentityServer.Core.Models;

namespace MyIdentityServer.Server
{
    
    public class ApplicationIdentityManagerService : AspNetIdentityManagerService<ApplicationUser, string, ApplicationRole, string>
    {
        public ApplicationIdentityManagerService(ApplicationUserManager userMgr, ApplicationRoleManager roleMgr)
            : base(userMgr, roleMgr)
        {
        }

        public override IdentityManagerMetadata GetStandardMetadata(bool includeAccountProperties = true)
        {
            var result = base.GetStandardMetadata(includeAccountProperties);
            
            //Ensure that the email property is included
            result.UserMetadata.CreateProperties = result.UserMetadata.CreateProperties
                .Union(new[]
                {
                    PropertyMetadata.FromProperty<ApplicationUser>(x => x.Email, type: Constants.ClaimTypes.Email, required: true)
                });
            return result;
        }
    }
}