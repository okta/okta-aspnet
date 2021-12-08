// <copyright file="OktaWebOptions.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Http;

namespace Okta.AspNet.Abstractions
{
    public class OktaWebOptions
    {
        public static readonly string DefaultAuthorizationServerId = "default";

        public static readonly TimeSpan DefaultClockSkew = TimeSpan.FromMinutes(2);

        /// <summary>
        /// Gets or sets the Okta domain, i.e https://dev-123456.oktapreview.com.
        /// </summary>
        /// <value>
        /// The Okta domain.
        /// </value>
        public string OktaDomain { get; set; }

        /// <summary>
        /// Gets or sets the Okta Authorization Server to use. The default value is <c>default</c>.
        /// </summary>
        /// <value>
        /// The Okta Authorization Server.
        /// </value>
        public string AuthorizationServerId { get; set; } = DefaultAuthorizationServerId;

        /// <summary>
        /// Gets or sets the clock skew allowed when validating tokens. The default value is 2 minutes.
        /// </summary>
        /// <value>
        /// The clock skew.
        /// </value>
        public TimeSpan ClockSkew { get; set; } = DefaultClockSkew;

        /// <summary>
        /// Gets or sets the URI of your organization's proxy server.  The default is <c>null</c>.
        /// </summary>
        /// <value>
        /// The URI of your organization's proxy server.  The default is <c>null</c>.
        /// </value>
        public ProxyConfiguration Proxy { get; set; }

        /// <summary>
        /// Gets or sets the HttpClientHandler used to communicate with Okta.
        /// </summary>
        /// <value>
        /// The HttpClientHandler used to communicate with Okta.
        /// </value>
        public HttpMessageHandler BackchannelHttpClientHandler { get; set; }

        /// <summary>
        /// Gets or sets timeout value in milliseconds for back channel communications with Okta.
        /// </summary>
        /// <value>
        /// Timeout value in milliseconds for back channel communications with Okta.
        /// </value>
        public TimeSpan BackchannelTimeout { get; set; } = TimeSpan.FromSeconds(60);
    }
}
