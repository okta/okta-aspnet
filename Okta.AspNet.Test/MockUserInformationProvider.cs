// <copyright file="MockUserInformationProvider.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Okta.AspNet.Test
{
    public class MockUserInformationProvider : IUserInformationProvider
    {
        private IList<Claim> _claims;

        public MockUserInformationProvider(IList<Claim> claims)
        {
            _claims = claims;
        }

        public Task EnrichIdentityViaUserInfoAsync(ClaimsIdentity subject, string accessToken)
        {
            subject.AddClaims(_claims);

            return Task.FromResult(0);
        }
    }
}
