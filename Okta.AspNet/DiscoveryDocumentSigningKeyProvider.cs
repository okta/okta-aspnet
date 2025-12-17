// <copyright file="DiscoveryDocumentSigningKeyProvider.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Okta.AspNet.Abstractions;

namespace Okta.AspNet
{
    internal sealed class DiscoveryDocumentSigningKeyProvider : IDiscoveryDocumentSigningKeyProvider
    {
        private readonly ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
        private readonly string _oktaDomain;
        private readonly string _authorizationServerId;

        public DiscoveryDocumentSigningKeyProvider(
            ConfigurationManager<OpenIdConnectConfiguration> configurationManager,
            string oktaDomain = null,
            string authorizationServerId = null)
        {
            _configurationManager = configurationManager;
            _oktaDomain = oktaDomain;
            _authorizationServerId = authorizationServerId;
        }

        public async Task<IEnumerable<SecurityKey>> GetSigningKeysAsync()
        {
            try
            {
                var discoveryDocument = await _configurationManager.GetConfigurationAsync().ConfigureAwait(false);
                return discoveryDocument.SigningKeys;
            }
            catch (Exception ex) when (!(ex is OktaOidcException))
            {
                throw OktaOidcException.CreateDiscoveryException(ex, _oktaDomain, _authorizationServerId);
            }
        }

        public IEnumerable<SecurityKey> GetSigningKeys()
        {
            try
            {
                var discoveryDocument = _configurationManager.GetConfigurationAsync().Result;
                return discoveryDocument.SigningKeys;
            }
            catch (AggregateException ex)
            {
                // Unwrap the AggregateException to get the actual exception
                var innerException = ex.InnerException ?? ex;
                if (innerException is OktaOidcException)
                {
                    throw innerException;
                }

                throw OktaOidcException.CreateDiscoveryException(innerException, _oktaDomain, _authorizationServerId);
            }
            catch (Exception ex) when (!(ex is OktaOidcException))
            {
                throw OktaOidcException.CreateDiscoveryException(ex, _oktaDomain, _authorizationServerId);
            }
        }
    }
}
