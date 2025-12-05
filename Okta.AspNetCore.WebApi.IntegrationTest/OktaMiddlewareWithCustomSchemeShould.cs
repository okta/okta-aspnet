using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace Okta.AspNetCore.WebApi.IntegrationTest
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
            BaseUrl = "http://localhost:58533";
            ProtectedEndpoint = $"{BaseUrl}/api/messages";
            
            _host = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<StartupUsingCustomScheme>()
                        .UseConfiguration(Configuration);
                })
                .Build();
            
            _server = _host.GetTestServer();
            _server.BaseAddress = new Uri(BaseUrl);
        }

        [Fact]
        public async Task Returns401WhenAccessToProtectedRouteWithoutTokenAsync()
        {
            using (var client = new HttpClient(_server.CreateHandler()))
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

            using (var client = new HttpClient(_server.CreateHandler()))
            {
                var response = await client.SendAsync(request);
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
            }
        }

        [Fact]
        public async Task InvokeCustomEventsAsync()
        {
            var accessToken = "thisIsAnInvalidToken";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, ProtectedEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            using (var client = new HttpClient(_server.CreateHandler()))
            {
                var response = await client.SendAsync(request);
                Assert.True(response.Headers.Contains("myCustomHeader"));
            }
        }

        public void Dispose()
        {
            _server.Dispose();
            _host.Dispose();
        }
    }
}
