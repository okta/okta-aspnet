using Microsoft.IdentityModel.Tokens;
using Okta.AspNet.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Okta.AspNet
{
    public class DefaultTokenValidationParameters : TokenValidationParameters
    {
        public DefaultTokenValidationParameters(OktaOptions options, string issuer)
        {
            RequireExpirationTime = true;
            RequireSignedTokens = true;

            ValidateIssuer = true;
            ValidIssuer = issuer;

            ValidateAudience = true;

            ValidateLifetime = true;
            ClockSkew = options.ClockSkew;

            ValidateIssuerSigningKey = true;
        }
    }
}
