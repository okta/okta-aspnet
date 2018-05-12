using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Okta.AspNet.Abstractions
{
    /// <summary>
    /// This class performs additional validation per Okta's best practices.
    /// https://developer.okta.com/code/dotnet/jwt-validation
    /// </summary>
    public class StrictTokenHandler : JwtSecurityTokenHandler
    {
        public string ClientId { get; set; }

        public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            // base.ValidateToken will throw if the token is invalid
            // in any way (according to validationParameters)
            var claimsPrincipal = base.ValidateToken(token, validationParameters, out validatedToken);
            var jwtToken = ReadJwtToken(token);

            var clientIdMatches = jwtToken.Payload.TryGetValue("cid", out var rawCid)
                && rawCid.ToString() == ClientId;

            if (!clientIdMatches)
            {
                throw new SecurityTokenValidationException("The cid claim was invalid.");
            }

            return claimsPrincipal;
        }
        
    }
}
