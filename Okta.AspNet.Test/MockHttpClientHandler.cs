// <copyright file="MockHttpClientHandler.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Okta.AspNet.Test
{
    public class MockHttpClientHandler : DelegatingHandler
    {
        private readonly string _response;
        private readonly HttpStatusCode _statusCode;

        public string Body { get; private set; }

        public int NumberOfCalls { get; private set; }

        public MockHttpClientHandler(string response = "{}", HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _response = response;
            _statusCode = statusCode;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            NumberOfCalls++;

            if (request.Content != null)
            {
                Body = await request.Content.ReadAsStringAsync();
            }

            return new HttpResponseMessage
            {
                StatusCode = _statusCode,
                Content = new StringContent(_response),
            };
        }
    }
}
