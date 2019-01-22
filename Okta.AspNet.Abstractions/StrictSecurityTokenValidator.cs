// <copyright file="StrictSecurityTokenValidator.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Okta.AspNet.Abstractions
{
    public sealed class StrictSecurityTokenValidator : ISecurityTokenValidator
    {
        private readonly JwtSecurityTokenHandler _handler;

        public StrictSecurityTokenValidator()
        {
            _handler = new StrictTokenHandler();
        }

        public bool CanValidateToken => _handler.CanValidateToken;

        public int MaximumTokenSizeInBytes
        {
            get => _handler.MaximumTokenSizeInBytes;
            set => throw new NotImplementedException();
        }

        public bool CanReadToken(string securityToken) => _handler.CanReadToken(securityToken);

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
            => _handler.ValidateToken(securityToken, validationParameters, out validatedToken);
    }
}
