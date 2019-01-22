// <copyright file="StrictTokenHandler.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Okta.AspNet.Abstractions
{
    /// <summary>
    /// This class performs additional validation per Okta's best practices.
    /// https://developer.okta.com/code/dotnet/jwt-validation.
    /// </summary>
    public sealed class StrictTokenHandler : JwtSecurityTokenHandler
    {
        public StrictTokenHandler()
        {
        }

        public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            // base.ValidateToken will throw if the token is invalid
            // in any way (according to validationParameters)
            var claimsPrincipal = base.ValidateToken(token, validationParameters, out validatedToken);
            var jwtToken = ReadJwtToken(token);

            if (jwtToken.Header?.Alg == null || jwtToken.Header?.Alg != SecurityAlgorithms.RsaSha256)
            {
                throw new SecurityTokenValidationException("The JWT token's signing algorithm must be RS256.");
            }

            return claimsPrincipal;
        }
    }
}
