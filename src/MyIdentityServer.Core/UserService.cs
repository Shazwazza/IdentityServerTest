using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using MyIdentityServer.Core.Models;

namespace MyIdentityServer.Core
{
    public class UserService : AspNetIdentityUserService<ApplicationUser, string>
    {
        public UserService(ApplicationUserManager userMgr)
            : base(userMgr)
        {
        }

        /// <summary>
        /// Ensures all of the required claims that we want are returned
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        protected override async Task<IEnumerable<Claim>> GetClaimsFromAccount(ApplicationUser user)
        {
            var claims = (await base.GetClaimsFromAccount(user)).ToList();
            if (!string.IsNullOrWhiteSpace(user.FirstName))
            {
                claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));
            }
            if (!string.IsNullOrWhiteSpace(user.LastName))
            {
                claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));
            }

            return claims;
        }
    }
}