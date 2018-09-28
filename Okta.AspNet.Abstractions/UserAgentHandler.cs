// <copyright file="UserAgentHandler.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Okta.AspNet.Abstractions
{
    public class UserAgentHandler : DelegatingHandler
    {
        private UserAgentBuilder _userAgentBuilder;

        public UserAgentHandler(string frameworkName, Version frameworkversion)
        {
            InnerHandler = new HttpClientHandler();
            _userAgentBuilder = new UserAgentBuilder(frameworkName, frameworkversion);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("user-agent", _userAgentBuilder.GetUserAgent());

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
