// <copyright file="StrictSecurityTokenHandler.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Okta.AspNet.Abstractions;

namespace Okta.AspNetCore
{
    public class StrictSecurityTokenHandler : ISecurityTokenValidator
    {
        private int _maxTokenSizeInBytes = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;
        private JwtSecurityTokenHandler _tokenHandler;

        public string ClientId { get; set; }

        public StrictSecurityTokenHandler()
        {
            _tokenHandler = new StrictTokenHandler();
        }

        public int MaximumTokenSizeInBytes
        {
            get
            {
                return _maxTokenSizeInBytes;
            }

            set
            {
                _maxTokenSizeInBytes = value;
            }
        }

        public bool CanReadToken(string securityToken)
        {
            return _tokenHandler.CanReadToken(securityToken);
        }

        public bool CanValidateToken
        {
            get
            {
                return true;
            }
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            // _tokenHandler.ValidateToken will throw if the token is invalid
            // in any way (according to validationParameters)
            return _tokenHandler.ValidateToken(securityToken, validationParameters, out validatedToken);
        }
    }
}
