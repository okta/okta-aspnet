// <copyright file="HttpClientBuilder.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Net.Http;
using Okta.AspNet.Abstractions;

namespace Okta.AspNet
{
    public class HttpClientBuilder
    {
        public static HttpClient CreateClient(OktaWebOptions options)
        {
            var httpClient = new HttpClient(new OktaHttpMessageHandler("okta-aspnet", typeof(OktaMiddlewareExtensions).Assembly.GetName().Version, options));

            httpClient.Timeout = options.BackchannelTimeout;
            httpClient.MaxResponseContentBufferSize = 1024 * 1024 * 10; // 10 MB

            return httpClient;
        }
    }
}
