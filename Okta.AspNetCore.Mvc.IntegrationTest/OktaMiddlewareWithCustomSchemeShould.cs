using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Okta.AspNetCore.Mvc.IntegrationTest
{
    public class OktaMiddlewareWithCustomSchemeShould : IDisposable
    {
        private readonly TestServer _server;

        private string BaseUrl { get; set; }

        private string ProtectedEndpoint { get; set; }

        public IConfiguration Configuration { get; set; }

        public OktaMiddlewareWithCustomSchemeShould()
        {
            Configuration = TestConfiguration.GetConfiguration();
            BaseUrl = "http://localhost:57451";
            ProtectedEndpoint = string.Format("{0}/AccountWithScheme/Claims", BaseUrl);
            _server = new TestServer(new WebHostBuilder()
            .UseStartup<StartupUsingCustomScheme>()
            .UseConfiguration(Configuration))
            {
                BaseAddress = new Uri(BaseUrl),
            };
        }

        [Fact]
        public async Task RedirectWhenAccessToProtectedRouteWithoutSignedInAsync()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, ProtectedEndpoint);
            using (var client = _server.CreateClient())
            {
                var response = await client.GetAsync(ProtectedEndpoint);
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.Found);
                Assert.StartsWith(Configuration["Okta:OktaDomain"], response.Headers.Location.AbsoluteUri);
            }
        }

        [Fact]
        public async Task IncludeIdpInAuthorizeUrlAsync()
        {
            var loginWithIdpEndpoint = string.Format("{0}/AccountWithScheme/LoginWithIdp", BaseUrl);
            using (var client = _server.CreateClient())
            {
                var response = await client.GetAsync(loginWithIdpEndpoint);
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.Found);
                Assert.Contains("idp=foo", response.Headers.Location.AbsoluteUri);
            }
        }

        [Fact]
        public async Task IncludeLoginHintInAuthorizeUrlAsync()
        {
            var loginWithLoginHintEndpoint = string.Format("{0}/AccountWithScheme/LoginWithLoginHint", BaseUrl);
            using (var client = _server.CreateClient())
            {
                var response = await client.GetAsync(loginWithLoginHintEndpoint);
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.Found);
                Assert.Contains("login_hint=foo", response.Headers.Location.AbsoluteUri);
            }
        }

        [Fact]
        public async Task CallCustomRedirectEventAfterInternalEventAsync()
        {
            var loginWithIdpEndpoint = string.Format("{0}/AccountWithScheme/LoginWithIdp", BaseUrl);
            using (var client = _server.CreateClient())
            {
                var response = await client.GetAsync(loginWithIdpEndpoint);
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.Found);
                Assert.Contains("idp=foo", response.Headers.Location.AbsoluteUri);
                Assert.Contains("myCustomParamKey=myCustomParamValue", response.Headers.Location.AbsoluteUri);
            }
        }

        public void Dispose()
        {
            _server.Dispose();
        }
    }
}
