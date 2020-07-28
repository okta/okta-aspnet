// <copyright file="DefaultProxy.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net;

namespace Okta.AspNet.Abstractions
{
    /// <summary>
    /// A simple implementation of <see cref="IWebProxy"/>.
    /// </summary>
    public sealed class DefaultProxy : IWebProxy
    {
        private readonly ICredentials _credentials;
        private readonly Uri _proxyUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultProxy"/> class.
        /// </summary>
        /// <param name="proxyConfiguration">The proxy configuration to use.</param>
        /// <param name="logger">The logging interface.</param>
        public DefaultProxy(ProxyConfiguration proxyConfiguration)
        {
            if (proxyConfiguration == null)
            {
                throw new ArgumentNullException(nameof(proxyConfiguration));
            }

            if (!string.IsNullOrEmpty(proxyConfiguration.Username) || !string.IsNullOrEmpty(proxyConfiguration.Password))
            {
                _credentials = new NetworkCredential(proxyConfiguration.Username, proxyConfiguration.Password);
            }

            var host = proxyConfiguration.Host;
            _proxyUri = new Uri(host, UriKind.Absolute);
            if (proxyConfiguration.Port > 0)
            {
                _proxyUri = new Uri($"{_proxyUri.Scheme}://{_proxyUri.Host}:{proxyConfiguration.Port}");
            }
        }

        /// <inheritdoc/>
        public ICredentials Credentials
        {
            get => _credentials;
            set => throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Uri GetProxy(Uri destination) => _proxyUri;

        /// <inheritdoc/>
        public bool IsBypassed(Uri host) => false;
    }
}
