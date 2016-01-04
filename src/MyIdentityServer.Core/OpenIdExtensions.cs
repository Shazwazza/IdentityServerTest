using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using IdentityModel;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

namespace MyIdentityServer.Core
{
    public static class OpenIdExtensions
    {
        public static OpenIdConnectAuthenticationOptions ConfigureMyIdentityServerOAuth(this IAppBuilder app, 
            string authority, string redirectUri, string clientId,
            string signInAsAuthType,
            string responseType,
            AuthenticationMode authMode,
            Action<SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions>, ClaimsIdentity> securityTokenValidated = null)
        {
            //TODO: Maybe not ideal that we put this logic here, but it is required for this provider to work...
            //Filters out the long claim names (see: https://identityserver.github.io/Documentation/docsv2/overview/mvcGettingStarted.html)

            //AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            AntiForgeryConfig.UniqueClaimTypeIdentifier = IdentityServer3.Core.Constants.ClaimTypes.Subject;
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            var options = new OpenIdConnectAuthenticationOptions
            {
                AuthenticationMode = authMode,


                Caption = "My Identity Server",
                AuthenticationType = "MyIdentityServer",
                Authority = authority,
                ClientId = clientId,
                RedirectUri = redirectUri,
                SignInAsAuthenticationType = signInAsAuthType,
                ResponseType = responseType,  
                Scope = "openid email profile roles",
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = n =>
                    {
                        //Applies "claims transformation", as seen: https://identityserver.github.io/Documentation/docsv2/overview/mvcGettingStarted.html
                        //So that only the necessary claims are kept for the user's ticket

                        var id = n.AuthenticationTicket.Identity;

                        // we want to keep first name, last name, subject and roles
                        var email = id.FindFirst(JwtClaimTypes.Email);
                        var givenName = id.FindFirst(JwtClaimTypes.GivenName);
                        var familyName = id.FindFirst(JwtClaimTypes.FamilyName);
                        var sub = id.FindFirst(JwtClaimTypes.Subject);
                        var roles = id.FindAll(JwtClaimTypes.Role);

                        // create new identity and set name and role claim type
                        var nid = new ClaimsIdentity(
                            id.AuthenticationType,
                            JwtClaimTypes.GivenName,
                            JwtClaimTypes.Role);

                        nid.AddClaim(email);
                        nid.AddClaim(givenName);
                        nid.AddClaim(familyName);
                        nid.AddClaim(sub);
                        nid.AddClaims(roles);

                        // keep the id_token for logout
                        nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                        //// add some other app specific claim
                        //nid.AddClaim(new Claim("app_specific", "some data"));

                        //Call the callback to allow for customization to the ClaimsIdentity
                        if (securityTokenValidated != null)
                        {
                            securityTokenValidated(n, nid);
                        }

                        n.AuthenticationTicket = new AuthenticationTicket(
                            nid,
                            n.AuthenticationTicket.Properties);
                        return Task.FromResult(0);
                    },
                    RedirectToIdentityProvider = n =>
                    {
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                        {
                            var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                            if (idTokenHint != null)
                            {
                                n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                            }
                        }

                        return Task.FromResult(0);
                    }
                }
            };
            app.UseOpenIdConnectAuthentication(options);
            return options;

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
        }
    }
}
