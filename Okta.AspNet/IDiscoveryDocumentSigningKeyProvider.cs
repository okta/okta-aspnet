// <copyright file="IDiscoveryDocumentSigningKeyProvider.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Okta.AspNet
{
    internal interface IDiscoveryDocumentSigningKeyProvider
    {
        Task<IEnumerable<SecurityKey>> GetSigningKeysAsync();

        IEnumerable<SecurityKey> GetSigningKeys();
    }
}
