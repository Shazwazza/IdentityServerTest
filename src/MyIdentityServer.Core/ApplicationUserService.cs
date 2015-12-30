using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using MyIdentityServer.Core.Models;

namespace MyIdentityServer.Core
{
    public class ApplicationUserService : AspNetIdentityUserService<ApplicationUser, string>
    {
        public ApplicationUserService(ApplicationUserManager userMgr)
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
            var fullName = new List<string>();
            if (!string.IsNullOrWhiteSpace(user.FirstName))
            {
                claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));
                fullName.Add(user.FirstName);
            }
            if (!string.IsNullOrWhiteSpace(user.LastName))
            {
                claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));
                fullName.Add(user.LastName);
            }

            //if (fullName.Any())
            //{
            //    //need this in order to populate the 'Display name'
            //    claims.Add(new Claim(JwtClaimTypes.Name, string.Join(" ", fullName.ToArray())));
            //}

            return claims;
        }
    }
}