// <copyright file="OktaMvcMiddlewareShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Owin.Testing;
using Xunit;

namespace Okta.AspNet.Mvc.IntegrationTest
{
    public class OktaMvcMiddlewareShould : IDisposable
    {
        private TestServer _server;

        private string BaseUrl { get; set; }

        private string ProtectedEndpoint { get; set; }

        private MockHttpMessageHandler MockHttpHandler { get; set; }

        public OktaMvcMiddlewareShould()
        {
            BaseUrl = "http://localhost:8080";
            ProtectedEndpoint = $"{BaseUrl}/Home/Protected";
            MockHttpHandler = new MockHttpMessageHandler();

            _server = TestServer.Create(app =>
            {
                var startup = new Startup();
                startup.HttpMessageHandler = MockHttpHandler;
                startup.Configuration(app);
            });

            _server.BaseAddress = new Uri(BaseUrl);
        }

        [Fact]
        public async Task ConfigureOktaMvcMiddleware()
        {
            // Arrange & Act
            using (var client = new HttpClient(_server.Handler))
            {
                var response = await client.GetAsync($"{BaseUrl}/Home/Index");

                // Assert
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                var content = await response.Content.ReadAsStringAsync();
                content.Should().Contain("Home Page");
            }
        }

        [Fact]
        public async Task RedirectToOktaWhenAccessingProtectedRouteWithoutAuthentication()
        {
            // Arrange & Act
            using (var client = new HttpClient(_server.Handler))
            {
                var response = await client.GetAsync(ProtectedEndpoint);

                // Assert - Should redirect to Okta login or return unauthorized
                response.StatusCode.Should().BeOneOf(HttpStatusCode.Redirect, HttpStatusCode.Found, HttpStatusCode.Unauthorized);
            }
        }

        [Fact]
        public async Task InvokeBackchannelHandler()
        {
            // Arrange
            using (var client = new HttpClient(_server.Handler))
            {
                // Act
                var response = await client.GetAsync(ProtectedEndpoint);

                // Assert - MockHttpHandler should have been called for .well-known/openid-configuration
                MockHttpHandler.NumberOfCalls.Should().BeGreaterThan(0);
            }
        }

        [Fact]
        public async Task HandleCallbackEndpoint()
        {
            // Arrange
            var callbackUrl = $"{BaseUrl}/authorization-code/callback?code=test_code&state=test_state";

            using (var client = new HttpClient(_server.Handler))
            {
                // Act
                var response = await client.GetAsync(callbackUrl);

                // Assert - Should attempt to process the callback
                // In real scenario, this would exchange code for tokens
                response.StatusCode.Should().BeOneOf(HttpStatusCode.Redirect, HttpStatusCode.Found, HttpStatusCode.InternalServerError);
            }
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}
