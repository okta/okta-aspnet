// <copyright file="MockHttpMessageHandler.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Okta.AspNet.Mvc.IntegrationTest
{
    public class MockHttpMessageHandler : DelegatingHandler
    {
        public int NumberOfCalls { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            NumberOfCalls++;

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(GetMockResponse(request.RequestUri.ToString()), Encoding.UTF8, "application/json"),
            };

            return Task.FromResult(responseMessage);
        }

        private string GetMockResponse(string url)
        {
            if (url.Contains("/.well-known/openid-configuration"))
            {
                return @"{
                    ""issuer"": ""https://test.okta.com"",
                    ""authorization_endpoint"": ""https://test.okta.com/oauth2/v1/authorize"",
                    ""token_endpoint"": ""https://test.okta.com/oauth2/v1/token"",
                    ""userinfo_endpoint"": ""https://test.okta.com/oauth2/v1/userinfo"",
                    ""jwks_uri"": ""https://test.okta.com/oauth2/v1/keys"",
                    ""end_session_endpoint"": ""https://test.okta.com/oauth2/v1/logout""
                }";
            }

            if (url.Contains("/oauth2/v1/keys"))
            {
                return @"{""keys"":[]}";
            }

            return "{}";
        }
    }
}
