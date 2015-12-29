using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using MyIdentityServer.Core;
using Owin;


namespace MyIdentityServer.Website
{
    //public static class Constants
    //{
    //    public const string BaseAddress = "https://localhost:44333/core";
    //    public const string AuthorizeEndpoint = BaseAddress + "/connect/authorize";
    //    public const string LogoutEndpoint = BaseAddress + "/connect/endsession";
    //    public const string TokenEndpoint = BaseAddress + "/connect/token";
    //    public const string UserInfoEndpoint = BaseAddress + "/connect/userinfo";
    //    public const string IdentityTokenValidationEndpoint = BaseAddress + "/connect/identitytokenvalidation";
    //    public const string TokenRevocationEndpoint = BaseAddress + "/connect/revocation";
    //    public const string AspNetWebApiSampleApi = "http://localhost:2727/";
    //}

    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {            

            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            
            // Configure the db context, user manager and signin manager to use a single instance per request
            //app.CreatePerOwinContext(ApplicationDbContext.Create);
            //app.CreatePerOwinContext<ApplicationUserManager>(CreateUserManager);
            //app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = "https://localhost:44301",
                ClientId = "mvc",
                RedirectUri = "https://localhost:44300/",
                ResponseType = "id_token",
                SignInAsAuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                Scope = "openid profile roles",
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = n =>
                    {
                        //Example of "claims transformation", as seen: https://identityserver.github.io/Documentation/docsv2/overview/mvcGettingStarted.html

                        //var id = n.AuthenticationTicket.Identity;

                        //// we want to keep first name, last name, subject and roles
                        //var givenName = id.FindFirst(Constants.ClaimTypes.GivenName);
                        //var familyName = id.FindFirst(Constants.ClaimTypes.FamilyName);
                        //var sub = id.FindFirst(Constants.ClaimTypes.Subject);
                        //var roles = id.FindAll(Constants.ClaimTypes.Role);

                        //// create new identity and set name and role claim type
                        //var nid = new ClaimsIdentity(
                        //    id.AuthenticationType,
                        //    Constants.ClaimTypes.GivenName,
                        //    Constants.ClaimTypes.Role);

                        //nid.AddClaim(givenName);
                        //nid.AddClaim(familyName);
                        //nid.AddClaim(sub);
                        //nid.AddClaims(roles);

                        //// add some other app specific claim
                        //nid.AddClaim(new Claim("app_specific", "some data"));

                        //n.AuthenticationTicket = new AuthenticationTicket(
                        //    nid,
                        //    n.AuthenticationTicket.Properties);
                        return Task.FromResult(0);
                    }
                }
            });

            //app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            //{
            //    AuthenticationType = "UaaS login",

            //    ClientId = "katanaclient",
            //    Authority = Constants.BaseAddress,
            //    RedirectUri = "https://localhost:44300/",
            //    PostLogoutRedirectUri = "https://localhost:44300/",
            //    ResponseType = "code id_token",
            //    Scope = "openid profile read write offline_access",

            //    SignInAsAuthenticationType = "Cookies",

            //    Notifications = new OpenIdConnectAuthenticationNotifications
            //    {
            //        AuthorizationCodeReceived = async n =>
            //        {
            //            // use the code to get the access and refresh token
            //            var tokenClient = new TokenClient(
            //                Constants.TokenEndpoint,
            //                "katanaclient",
            //                "secret");

            //            var tokenResponse = await tokenClient.RequestAuthorizationCodeAsync(
            //                n.Code, n.RedirectUri);

            //            // use the access token to retrieve claims from userinfo
            //            var userInfoClient = new UserInfoClient(
            //                new Uri(Constants.UserInfoEndpoint),
            //                tokenResponse.AccessToken);

            //            var userInfoResponse = await userInfoClient.GetAsync();

            //            // create new identity
            //            var id = new ClaimsIdentity(n.AuthenticationTicket.Identity.AuthenticationType);
            //            id.AddClaims(userInfoResponse.GetClaimsIdentity().Claims);

            //            id.AddClaim(new Claim("access_token", tokenResponse.AccessToken));
            //            id.AddClaim(new Claim("expires_at", DateTime.Now.AddSeconds(tokenResponse.ExpiresIn).ToLocalTime().ToString(CultureInfo.InvariantCulture)));
            //            id.AddClaim(new Claim("refresh_token", tokenResponse.RefreshToken));
            //            id.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));
            //            id.AddClaim(new Claim("sid", n.AuthenticationTicket.Identity.FindFirst("sid").Value));

            //            n.AuthenticationTicket = new AuthenticationTicket(
            //                new ClaimsIdentity(id.Claims, n.AuthenticationTicket.Identity.AuthenticationType),
            //                n.AuthenticationTicket.Properties);
            //        },

            //        RedirectToIdentityProvider = n =>
            //        {
            //            // if signing out, add the id_token_hint
            //            if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
            //            {
            //                var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

            //                if (idTokenHint != null)
            //                {
            //                    n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
            //                }

            //            }

            //            return Task.FromResult(0);
            //        }
            //    }
            //});

            //// Enable the application to use a cookie to store information for the signed in user
            //// and to use a cookie to temporarily store information about a user logging in with a third party login provider
            //// Configure the sign in cookie
            //app.UseCookieAuthentication(new CookieAuthenticationOptions
            //{
            //    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
            //    LoginPath = new PathString("/Account/Login"),
            //    Provider = new CookieAuthenticationProvider
            //    {
            //        // Enables the application to validate the security stamp when the user logs in.
            //        // This is a security feature which is used when you change a password or add an external login to your account.  
            //        OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
            //            validateInterval: TimeSpan.FromMinutes(30),
            //            regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
            //    }
            //});            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
                        
        }

        
    }
}