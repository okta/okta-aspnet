using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Okta.AspNetCore.Mvc.IntegrationTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Okta.AspNetCore.Mvc.IntegrationTest
{
    public class MiddlewareShould : IDisposable
    {
        private readonly TestServer _server;
        private string BaseUrl { get; set; }
        private string ProtectedEndpoint { get; set; }
        public IConfiguration Configuration { get; set; }

        public MiddlewareShould()
        {
            Configuration = TestConfiguration.GetConfiguration();
            BaseUrl = "http://localhost:57451";
            ProtectedEndpoint = String.Format("{0}/Account/Claims", BaseUrl);
            _server = new TestServer(new WebHostBuilder()
            .UseStartup<Startup>()
            .UseConfiguration(Configuration));            
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
                Assert.StartsWith(Configuration["Okta:OrgUrl"], response.Headers.Location.AbsoluteUri);
            }
        }

        public void Dispose()
        {
            _server.Dispose();
        }
    }
}
