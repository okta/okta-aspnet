using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Okta.AspNetCore.Mvc.IntegrationTest
{
    public sealed class OktaMiddlewareWithCustomSchemeShould : IDisposable
    {
        private readonly TestServer _server;
        private readonly IHost _host;

        private string BaseUrl { get; set; }

        private string ProtectedEndpoint { get; set; }

        public IConfiguration Configuration { get; set; }

        public OktaMiddlewareWithCustomSchemeShould()
        {
            Configuration = TestConfiguration.GetConfiguration();
            BaseUrl = "http://localhost:57451";
            ProtectedEndpoint = string.Format("{0}/AccountWithScheme/Claims", BaseUrl);
            
            _host = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .UseStartup<StartupUsingCustomScheme>()
                        .UseConfiguration(Configuration);
                })
                .Build();
            
            _host.Start();
            _server = _host.GetTestServer();
            _server.BaseAddress = new Uri(BaseUrl);
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

#if NET9_0_OR_GREATER
        [Fact]
        public async Task NotIncludeRequestUriWhenPushedAuthorizationBehaviorIsDisabledWithCustomScheme()
        {
            // This test verifies that when PushedAuthorizationBehavior is set to Disable with a custom scheme,
            // the authorization parameters are sent directly in the URL (not via PAR endpoint)
            using (var client = _server.CreateClient())
            {
                var response = await client.GetAsync(ProtectedEndpoint);
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.Found);
                
                // When PAR is disabled, the authorize URL should NOT contain request_uri parameter
                Assert.DoesNotContain("request_uri=", response.Headers.Location.AbsoluteUri);
                
                // Instead, it should contain the actual parameters in the URL
                Assert.Contains("client_id=", response.Headers.Location.AbsoluteUri);
                Assert.Contains("redirect_uri=", response.Headers.Location.AbsoluteUri);
                Assert.Contains("response_type=code", response.Headers.Location.AbsoluteUri);
            }
        }
#endif

        public void Dispose()
        {
            _server.Dispose();
            _host.Dispose();
        }
    }
}
