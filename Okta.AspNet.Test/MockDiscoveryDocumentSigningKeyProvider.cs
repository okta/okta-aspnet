// <copyright file="MockDiscoveryDocumentSigningKeyProvider.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;

namespace Okta.AspNet.Test
{
    public class MockDiscoveryDocumentSigningKeyProvider : IDiscoveryDocumentSigningKeyProvider
    {
        public int NumberOfCalls { get; set; }

        public IEnumerable<SecurityKey> GetSigningKeys()
        {
            NumberOfCalls++;
            SecurityKey mockKey = Substitute.For<SecurityKey>();
            return new List<SecurityKey>() { mockKey };
        }

        public Task<IEnumerable<SecurityKey>> GetSigningKeysAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
