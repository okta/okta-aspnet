using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Okta.AspNetCore.Mvc.IntegrationTest
{
    public class MiddlewareShould
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public MiddlewareShould()
        {
            _server = new TestServer(new WebHostBuilder()
            .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public void FailWhenAccessToProtectedRouteWithoutSignedIn()
        {

        }
    }
}
