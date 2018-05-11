using Microsoft.Extensions.DependencyInjection;
using Okta.AspNet.Abstractions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;

namespace Okta.AspNetCore
{
    public static class OktaAuthenticationOptionsExtensions
    {
        public static AuthenticationBuilder AddOktaMvc(this AuthenticationBuilder builder, OktaMvcOptions oktaOptions)
        {
            var issuer = UrlHelper.CreateIssuerUrl(oktaOptions.OrgUrl, oktaOptions.AuthorizationServerId);

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            builder.AddOpenIdConnect(options => 
            {
                options.ClientId = oktaOptions.ClientId;
                options.ClientSecret = oktaOptions.ClientSecret;
                options.Authority = issuer;
                options.CallbackPath = new PathString("/authorization-code/callback");
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.GetClaimsFromUserInfoEndpoint = oktaOptions.GetClaimsFromUserInfoEndpoint;
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.SaveTokens = true;
                options.UseTokenLifetime = false;
                options.SecurityTokenValidator = new StrictSecurityTokenHandler() { ClientId = oktaOptions.ClientId};
                options.TokenValidationParameters = new DefaultTokenValidationParameters(oktaOptions, issuer)
                {
                    NameClaimType = "name",
                };
            });

            return builder;
        }

        public static AuthenticationBuilder AddOktaWebApi(this AuthenticationBuilder builder, OktaWebApiOptions oktaOptions)
        {
            var issuer = UrlHelper.CreateIssuerUrl(oktaOptions.OrgUrl, oktaOptions.AuthorizationServerId);
            var tokenValidationParameters = new DefaultTokenValidationParameters(oktaOptions, issuer)
            {
                ValidAudience = oktaOptions.Audience,
            };

            builder.AddJwtBearer(options =>
            {
                options.Authority = issuer;
                options.TokenValidationParameters = tokenValidationParameters;
                options.SecurityTokenValidators.Add(new StrictSecurityTokenHandler()
                    {
                        ClientId = oktaOptions.ClientId
                    }
                );
                
            });

            return builder;
        }
    }
}
