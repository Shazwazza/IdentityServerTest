using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
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

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            //Set up the OAuth provider with our identity server
            app.ConfigureMyIdentityServerOAuth(
                authority: "https://localhost:44301",
                redirectUri: "https://localhost:44300/",
                clientId: "portal",
                signInAsAuthType: CookieAuthenticationDefaults.AuthenticationType,
                //Since this is the primary auth type for this website, this auth type is Active
                authMode:AuthenticationMode.Active, 
                responseType: "id_token");

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