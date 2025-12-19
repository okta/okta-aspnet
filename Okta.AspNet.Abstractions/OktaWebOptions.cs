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

        private string _issuer;

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
        /// Gets or sets the full Issuer URL. This is an alternative to setting <see cref="OktaDomain"/> and <see cref="AuthorizationServerId"/> separately.
        /// When set, the <see cref="OktaDomain"/> and <see cref="AuthorizationServerId"/> properties will be automatically parsed from this value.
        /// Example: https://dev-123456.okta.com/oauth2/default
        /// </summary>
        /// <value>
        /// The full issuer URL.
        /// </value>
        public string Issuer
        {
            get => _issuer;
            set
            {
                _issuer = value;
                if (!string.IsNullOrEmpty(value))
                {
                    ParseIssuerUrl(value);
                }
            }
        }

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
        /// Gets or sets the HttpMessageHandler used to communicate with Okta.
        /// </summary>
        /// <value>
        /// The HttpMessageHandler used to communicate with Okta.
        /// </value>
        public HttpMessageHandler BackchannelHttpClientHandler { get; set; }

        /// <summary>
        /// Gets or sets timeout value in milliseconds for back channel communications with Okta.
        /// </summary>
        /// <value>
        /// Timeout value in milliseconds for back channel communications with Okta.
        /// </value>
        public TimeSpan BackchannelTimeout { get; set; } = TimeSpan.FromSeconds(120);

        /// <summary>
        /// Parses the issuer URL and extracts the OktaDomain and AuthorizationServerId.
        /// </summary>
        /// <param name="issuerUrl">The full issuer URL (e.g., https://dev-123456.okta.com/oauth2/default).</param>
        private void ParseIssuerUrl(string issuerUrl)
        {
            if (string.IsNullOrEmpty(issuerUrl))
            {
                return;
            }

            // Try to parse the issuer URL
            if (!Uri.TryCreate(issuerUrl, UriKind.Absolute, out var uri))
            {
                return;
            }

            // Extract the base domain (scheme + host + port if non-default)
            var baseUrl = uri.GetLeftPart(UriPartial.Authority);

            // Check if the path contains /oauth2/{authServerId}
            var path = uri.AbsolutePath.TrimEnd('/');
            if (path.StartsWith("/oauth2/", StringComparison.OrdinalIgnoreCase) && path.Length > 8)
            {
                // Extract the authorization server ID from the path
                var authServerId = path.Substring(8); // Remove "/oauth2/"
                OktaDomain = baseUrl;
                AuthorizationServerId = authServerId;
            }
            else if (string.IsNullOrEmpty(path) || path == "/")
            {
                // No authorization server in the URL, use org authorization server
                OktaDomain = baseUrl;
                AuthorizationServerId = string.Empty;
            }
            else
            {
                // Unknown path format, just use the full URL as the domain
                OktaDomain = baseUrl;
            }
        }
    }
}
