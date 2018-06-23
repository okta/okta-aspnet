// <copyright file="DefaultTokenValidationParameters.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.IdentityModel.Tokens;

namespace Okta.AspNet.Abstractions
{
    public class DefaultTokenValidationParameters : TokenValidationParameters
    {
        public DefaultTokenValidationParameters(OktaWebOptions options, string issuer)
        {
            RequireExpirationTime = true;
            RequireSignedTokens = true;
            ValidateIssuer = true;
            ValidIssuer = issuer;
            ValidateAudience = true;
            ValidateIssuerSigningKey = true;
            ValidateLifetime = true;
            ClockSkew = options.ClockSkew;
        }
    }
}
