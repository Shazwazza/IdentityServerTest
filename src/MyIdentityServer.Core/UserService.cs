using MyIdentityServer.Core.Models;

namespace MyIdentityServer.Core
{
    public class UserService : AspNetIdentityUserService<ApplicationUser, string>
    {
        public UserService(ApplicationUserManager userMgr)
            : base(userMgr)
        {
        }

        //protected override async Task<IEnumerable<Claim>> GetClaimsFromAccount(ApplicationUser user)
        //{
        //    var claims = (await base.GetClaimsFromAccount(user)).ToList();
        //    if (!string.IsNullOrWhiteSpace(user.FirstName))
        //    {
        //        claims.Add(new Claim("given_name", user.FirstName));
        //    }
        //    if (!string.IsNullOrWhiteSpace(user.LastName))
        //    {
        //        claims.Add(new Claim("family_name", user.LastName));
        //    }

        //    return claims;
        //}
    }
}