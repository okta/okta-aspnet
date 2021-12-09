// <copyright file="OktaHttpMessageHandler.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Okta.AspNet.Abstractions
{
    /// <summary>
    /// A delegating message handler used to make Okta specific modifications to requests.
    /// </summary>
    public class OktaHttpMessageHandler : DelegatingHandler
    {
        private readonly Lazy<string> _userAgent;

        public OktaHttpMessageHandler(string frameworkName, Version frameworkVersion, OktaWebOptions oktaWebOptions)
        {
            _userAgent = new Lazy<string>(() => new UserAgentBuilder(frameworkName, frameworkVersion).GetUserAgent());
            InnerHandler = oktaWebOptions.BackchannelHttpClientHandler ?? new HttpClientHandler();

            // If a backchannel handler is provided, then the proxy config is not overwritten
            if (oktaWebOptions.BackchannelHttpClientHandler == null && oktaWebOptions.Proxy != null)
            {
                ((HttpClientHandler)InnerHandler).Proxy = new DefaultProxy(oktaWebOptions.Proxy);
            }
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.UserAgent.ParseAdd(_userAgent.Value);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
