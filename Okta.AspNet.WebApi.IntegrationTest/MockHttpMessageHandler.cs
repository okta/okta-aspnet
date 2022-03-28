// <copyright file="MockHttpMessageHandler.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Okta.AspNet.WebApi.IntegrationTest
{
    public class MockHttpMessageHandler : DelegatingHandler
    {
        public int NumberOfCalls { get; private set; }

        public MockHttpMessageHandler()
        {
            NumberOfCalls = 0;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            NumberOfCalls++;
            // base.SendAsync calls the inner handler
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
