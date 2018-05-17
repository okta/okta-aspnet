// <copyright file="DiscoveryDocumentSigningKeyProvider.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Okta.AspNet
{
    internal sealed class DiscoveryDocumentSigningKeyProvider
    {
        private readonly ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;

        public DiscoveryDocumentSigningKeyProvider(
            ConfigurationManager<OpenIdConnectConfiguration> configurationManager)
        {
            _configurationManager = configurationManager;
        }

        public async Task<ICollection<SecurityKey>> GetSigningKeysAsync()
        {
            var discoveryDocument = await _configurationManager.GetConfigurationAsync().ConfigureAwait(false);
            return discoveryDocument.SigningKeys;
        }
    }
}
