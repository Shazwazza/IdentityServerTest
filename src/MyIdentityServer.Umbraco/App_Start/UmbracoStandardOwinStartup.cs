using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using IdentityModel;
using IdentityServer3.AccessTokenValidation;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using MyIdentityServer.Core;
using MyIdentityServer.Umbraco;
using Owin;
using Umbraco.Web;
using Umbraco.Web.Security.Identity;

//To use this startup class, change the appSetting value in the web.config called 
// "owin:appStartup" to be "UmbracoStandardOwinStartup"

[assembly: OwinStartup("UmbracoStandardOwinStartup", typeof(UmbracoStandardOwinStartup))]

namespace MyIdentityServer.Umbraco
{
    /// <summary>
    /// The standard way to configure OWIN for Umbraco
    /// </summary>
    /// <remarks>
    /// The startup type is specified in appSettings under owin:appStartup - change it to "StandardUmbracoStartup" to use this class
    /// </remarks>
    public class UmbracoStandardOwinStartup : UmbracoDefaultOwinStartup
    {
        public override void Configuration(IAppBuilder app)
        {
            //ensure the default options are configured
            base.Configuration(app);

            //Set up the OAuth provider with our identity server
            var options = app.ConfigureMyIdentityServerOAuth(
                authority: "https://localhost:44301",
                redirectUri: "https://localhost:44302/",
                clientId: "Site-16927631-722A-4712-8175-E5B42C6FCB98",
                signInAsAuthType: global::Umbraco.Core.Constants.Security.BackOfficeExternalAuthenticationType,
                responseType: "id_token",
                //ensure it's passive for Umbraco - because Umbraco has it's own auth system, we only want this to
                //be enabled when it is specifically asked for.
                authMode: AuthenticationMode.Passive,
                securityTokenValidated: (notification, identity) =>
                {
                    var id = notification.AuthenticationTicket.Identity;
                    var email = id.FindFirst(JwtClaimTypes.Email);
                    var issuer = id.FindFirst(JwtClaimTypes.Issuer);
                    var sub = id.FindFirst(JwtClaimTypes.Subject);

                    //This is important, this is required to work with the Umbraco back office identity system, Umbraco
                    // uses this claim to find and create the external UserLoginInfo object,
                    // we are also manually setting the 'Issuer' here to be the same as the auth type so that Umbraco knows
                    // how to wire everything up since the Issuer by default from identity server is the server address.
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier,
                        //The unique ID of the user
                        sub.Value,
                        //The issuer needs to match the Auth type
                        issuer: $"{(global::Umbraco.Core.Constants.Security.BackOfficeExternalAuthenticationTypePrefix)}MyIdentityServer",
                        originalIssuer: issuer.Value,
                        valueType: "http://www.w3.org/2001/XMLSchema#string"));

                    //This is important, this is required to work with the Umbraco back office identity system since this
                    // is the claim it looks for when auto-linking accounts
                    identity.AddClaim(new Claim(ClaimTypes.Email, email.Value));
                });
            options.ForUmbracoBackOffice("btn-microsoft", "fa-windows");
            options.SetExternalSignInAutoLinkOptions(new ExternalSignInAutoLinkOptions(
                autoLinkExternalAccount:true));

            //Enable token auth from Identity server
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = "https://localhost:44301",
                RequiredScopes = new[] { "sampleApi" }
            });

            /* 
                 * Configure external logins for the back office:
                 * 
                 * Depending on the authentication sources you would like to enable, you will need to install 
                 * certain Nuget packages. 
                 * 
                 * For Google auth:					    Install-Package UmbracoCms.IdentityExtensions.Google
                 * For Facebook auth:					Install-Package UmbracoCms.IdentityExtensions.Facebook
                 * For Microsoft auth:					Install-Package UmbracoCms.IdentityExtensions.Microsoft
                 * For Azure ActiveDirectory auth:		Install-Package UmbracoCms.IdentityExtensions.AzureActiveDirectory
                 * 
                 * There are many more providers such as Twitter, Yahoo, ActiveDirectory, etc... most information can
                 * be found here: http://www.asp.net/web-api/overview/security/external-authentication-services
                 * 
                 * For sample code on using external providers with the Umbraco back office, install one of the 
                 * packages listed above to review it's code samples 
                 *  
                 */

            /*
                 * To configure a simple auth token server for the back office:
                 *             
                 * By default the CORS policy is to allow all requests
                 * 
                 *      app.UseUmbracoBackOfficeTokenAuth(new BackOfficeAuthServerProviderOptions());
                 *      
                 * If you want to have a custom CORS policy for the token server you can provide
                 * a custom CORS policy, example: 
                 * 
                 *      app.UseUmbracoBackOfficeTokenAuth(
                 *          new BackOfficeAuthServerProviderOptions()
                 *              {
                 *             		//Modify the CorsPolicy as required
                 *                  CorsPolicy = new CorsPolicy()
                 *                  {
                 *                      AllowAnyHeader = true,
                 *                      AllowAnyMethod = true,
                 *                      Origins = { "http://mywebsite.com" }                
                 *                  }
                 *              });
                 */

        }
    }
}
