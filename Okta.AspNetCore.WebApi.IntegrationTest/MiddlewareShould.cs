// <copyright file="MiddlewareShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Okta.AspNetCore.WebApi.IntegrationTest
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
            BaseUrl = "http://localhost:58533";
            ProtectedEndpoint = $"{BaseUrl}/api/messages";
            _server = new TestServer(new WebHostBuilder()
            .UseStartup<Startup>()
            .UseConfiguration(Configuration));
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

        public void Dispose()
        {
            _server.Dispose();
        }
    }
}
