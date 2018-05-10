using Microsoft.IdentityModel.Tokens;
using Okta.AspNet.Abstractions;

namespace Okta.AspNet.Abstractions
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
            ValidateIssuerSigningKey = true;
            ValidateLifetime = true;
            ClockSkew = options.ClockSkew;
        }
    }
}
