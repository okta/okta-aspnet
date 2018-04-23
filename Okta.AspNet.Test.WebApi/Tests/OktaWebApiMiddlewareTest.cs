using Microsoft.Owin.Testing;
using NUnit.Framework;
using Owin;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace Okta.AspNet.Test.WebApi.Tests
{
    [TestFixture]
    public class OktaWebApiMiddlewareTest
    {
        private TestServer _server;
        private string BaseUrl { get; set; }
        private string ProtectedEndpoint { get; set; }

        [OneTimeSetUp]
        public void FixtureInit()
        {
            BaseUrl = "http://localhost:8080";
            ProtectedEndpoint = String.Format("{0}/api/messages", BaseUrl);

            _server = TestServer.Create(app =>
            {
                var startup = new Startup();
                startup.Configuration(app);

                HttpConfiguration config = new HttpConfiguration();
                config.Services.Replace(typeof(IAssembliesResolver), new TestWebApiResolver());
                config.MapHttpAttributeRoutes();
                app.UseWebApi(config);
            });

            _server.BaseAddress = new Uri(BaseUrl);
        }

        [Test]
        public async Task TestReturns401WhenAccessToProtectedRouteWithoutTokenAsync()
        {
            using (var client = new HttpClient(_server.Handler))
            {
                var response = await client.GetAsync(ProtectedEndpoint);
                Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
            }
        }

        [Test]
        public async Task TestReturns401WhenAccessToProtectedRouteWithInvalidTokenAsync()
        {
            var accessToken = "thisIsAnInvalidToken";
            using (var client = new HttpClient(_server.Handler))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.GetAsync(ProtectedEndpoint);
                Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
            }
        }

        [OneTimeTearDown]
        public void FixtureDispose()
        {
            _server.Dispose();
        }

    }
}