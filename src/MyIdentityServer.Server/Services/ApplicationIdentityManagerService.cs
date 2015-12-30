using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityManager;
using IdentityManager.AspNetIdentity;
using IdentityModel;
using MyIdentityServer.Core;
using MyIdentityServer.Core.Models;

namespace MyIdentityServer.Server.Services
{
    
    public class ApplicationIdentityManagerService : AspNetIdentityManagerService<ApplicationUser, string, ApplicationRole, string>
    {
        public ApplicationIdentityManagerService(ApplicationUserManager userMgr, ApplicationRoleManager roleMgr)
            : base(userMgr, roleMgr)
        {
        }

        /// <summary>
        /// Returns the correct meta data for the user for creating them
        /// </summary>
        /// <param name="includeAccountProperties"></param>
        /// <returns></returns>
        public override IdentityManagerMetadata GetStandardMetadata(bool includeAccountProperties = true)
        {
            var result = base.GetStandardMetadata(includeAccountProperties);
            
            //Ensure that the email property is included
            result.UserMetadata.CreateProperties = result.UserMetadata.CreateProperties
                .Union(new[]
                {
                    PropertyMetadata.FromProperty<ApplicationUser>(x => x.Email, type: JwtClaimTypes.Email, required: true),
                    PropertyMetadata.FromProperty<ApplicationUser>(x => x.FirstName, type: JwtClaimTypes.GivenName, required: true),
                    PropertyMetadata.FromProperty<ApplicationUser>(x => x.LastName, type: JwtClaimTypes.FamilyName, required: true)
                });

            return result;
        }
       
    }
}