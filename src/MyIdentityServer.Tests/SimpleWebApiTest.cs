using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using IdentityModel.Client;
using IdentityServer3.AccessTokenValidation;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services.InMemory;
using Microsoft.Owin.Testing;
using MyIdentityServer.Tests.TestHelpers;
using NUnit.Framework;
using Owin;

namespace MyIdentityServer.Tests
{
    [TestFixture]
    public class SimpleWebApiTest
    {
        [Test]
        public async Task Acquire_Token()
        {
            using (var server = TestServer.Create(Configuration))
            {
                var token = await GetClientTokenAsync(server.HttpClient);
                
                Assert.IsFalse(token.IsError);
                Assert.IsFalse(token.IsHttpError);
                Assert.IsNotEmpty(token.Raw);
            }
        }

        [Test]
        public async Task Endpoint_With_Token()
        {
            TokenResponse token = null;
            using (var server = TestServer.Create(Configuration))
            {
                token = await GetClientTokenAsync(server.HttpClient);
            }
            using (var server = TestServer.Create(Configuration))
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://localhost/test"),
                    Method = HttpMethod.Get,
                    Headers =
                    {
                        Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken)
                    }
                };                
                Console.WriteLine(request);
                var result = await server.HttpClient.SendAsync(request);
                Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            }
        }

        [Test]
        public async Task Endpoint_Without_Token()
        {
            using (var server = TestServer.Create(Configuration))
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://localhost/test"),
                    Method = HttpMethod.Get,
                };
                
                Console.WriteLine(request);

                var result = await server.HttpClient.SendAsync(request);
                Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
            }
        }

        public void Configuration(IAppBuilder app)
        {
            

            app.Map("/identity", idsrvApp =>
            {
                //Enable identity server
                var options = new IdentityServerOptions
                {
                    Factory = new IdentityServerServiceFactory()
                                .UseInMemoryClients(Clients.Get())
                                .UseInMemoryScopes(Scopes.Get())
                                .UseInMemoryUsers(new List<InMemoryUser>()),
                    SigningCertificate = LoadCertificate(),
                    RequireSsl = false
                };

                idsrvApp.UseIdentityServer(options);
                //idsrvApp.UseIdentityServer(new IdentityServerOptions
                //{
                //    SiteName = "Embedded IdentityServer",
                //    SigningCertificate = LoadCertificate(),

                //    Factory = new IdentityServerServiceFactory()
                //        .UseInMemoryClients(Clients.Get())
                //        .UseInMemoryScopes(Scopes.Get())
                //        .UseInMemoryUsers(new List<InMemoryUser>())
                //});

                //Accept access tokens from identityserver and require a scope of 'api1'
                idsrvApp.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
                {
                    Authority = "https://localhost/",
                    ValidationMode = ValidationMode.Both,
                    RequiredScopes = new[] { "api1" }
                });
            });
            

            


            // configure web api
            var config = new HttpConfiguration();
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            config.MapHttpAttributeRoutes();

            // require authentication for all controllers
            config.Filters.Add(new AuthorizeAttribute());

            app.UseWebApi(config);
        }

        X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(
                string.Format(@"{0}\idsrv3test.pfx", AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");
        }

        private async Task<TokenResponse> GetClientTokenAsync(HttpClient client)
        {
            var tokenClient = new TestTokenClient(
                client,
                "https://localhost/connect/token",
                "silicon",
                "F621F470-9731-4A25-80EF-67A6F7C5F4B8");

            return await tokenClient.RequestClientCredentialsAsync("api1");
        }

        /// <summary>
        /// Need this so that we can use our testing HttpClient
        /// </summary>
        public class TestTokenClient : TokenClient
        {
            public TestTokenClient(HttpClient client, string address, string clientId, string clientSecret, AuthenticationStyle style = AuthenticationStyle.BasicAuthentication)
                : base(address, clientId, clientSecret, style)
            {
                _client = client;
                _client.BaseAddress = new Uri(address);
                _client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(clientId, clientSecret);
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }


    }


}
