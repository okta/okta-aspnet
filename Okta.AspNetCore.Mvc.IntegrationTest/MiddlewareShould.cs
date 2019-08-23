// <copyright file="MiddlewareShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Okta.AspNetCore.Mvc.IntegrationTest.Models;
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
            ProtectedEndpoint = string.Format("{0}/Account/Claims", BaseUrl);
            _server = new TestServer(new WebHostBuilder()
            .UseStartup<Startup>()
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
            var loginWithIdpEndpoint = string.Format("{0}/Account/LoginWithIdp", BaseUrl);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, loginWithIdpEndpoint);
            using (var client = _server.CreateClient())
            {
                var response = await client.GetAsync(loginWithIdpEndpoint);
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.Found);
                Assert.Contains("idp=foo", response.Headers.Location.AbsoluteUri);
            }
        }

        public void Dispose()
        {
            _server.Dispose();
        }
    }
}
