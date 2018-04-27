using Microsoft.Owin.Testing;
using Owin;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Xunit;

namespace Okta.AspNet.Test.WebApi.Tests
{
    public class MiddlewareShould : IDisposable
    {
        private TestServer _server;
        private string BaseUrl { get; set; }
        private string ProtectedEndpoint { get; set; }

        public MiddlewareShould()
        {
            BaseUrl = "http://localhost:8080";
            ProtectedEndpoint = String.Format("{0}/api/messages", BaseUrl);

            _server = TestServer.Create(app =>
            {
                var startup = new Startup();
                startup.Configuration(app);

                HttpConfiguration config = new HttpConfiguration();
                config.Services.Replace(typeof(IAssembliesResolver), new WebApiResolver());
                config.MapHttpAttributeRoutes();
                app.UseWebApi(config);
            });

            _server.BaseAddress = new Uri(BaseUrl);
        }
        
        [Fact]
        public async Task Returns401WhenAccessToProtectedRouteWithoutTokenAsync()
        {
            using (var client = new HttpClient(_server.Handler))
            {
                var response = await client.GetAsync(ProtectedEndpoint);
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
            }
        }

        [Fact]
        public async Task Returns401WhenAccessToProtectedRouteWithInvalidTokenAsync()
        {
            var accessToken = "thisIsAnInvalidToken";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, ProtectedEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            using (var client = new HttpClient(_server.Handler))
            {
                var response = await client.SendAsync(request);
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
            }
        }
        
        public void Dispose()
        {
            _server.Dispose();
        }
    }
}