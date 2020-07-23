// <copyright file="DiscoveryDocumentSigningKeyProviderShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using Xunit;

namespace Okta.AspNet.Test
{
    public class DiscoveryDocumentSigningKeyProviderShould
    {
        [Fact]
        public async Task CacheSigningKeysAfterRetrieval()
        {
            var provider = new MockDiscoveryDocumentSigningKeyProvider();

            // The DiscoveryDocumentCachingSigningKeyProvider's constructor queue the first call to retrieve keys in the thread pool
            var cachingProvider = new DiscoveryDocumentCachingSigningKeyProvider(provider);

            // Give the thread pool some time to execute the keys retrieval
            await Task.Delay(3000);

            // First call to get SigningKeys
            cachingProvider.SigningKeys.Should().NotBeNull();

            // Second call
            var keys1 = cachingProvider.SigningKeys;

            // Third call
            var keys2 = cachingProvider.SigningKeys;

            // Although we called SigningKeys 3 times, it will only execute GetSigningKeys once because after retrieving the keys they are cached for 1 day.
            provider.NumberOfCalls.Should().Be(1);
        }
    }
}
