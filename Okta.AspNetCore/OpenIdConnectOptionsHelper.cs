using System.Linq;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Okta.AspNet.Abstractions;

namespace Okta.AspNetCore
{
    public class OpenIdConnectOptionsHelper
    {
        public static void InitializeOpenIdConnectOptions(OktaMvcOptions options, string issuer, OpenIdConnectEvents events, OpenIdConnectOptions oidcOptions)
        {
            oidcOptions.ClientId = options.ClientId;
            oidcOptions.ClientSecret = options.ClientSecret;
            oidcOptions.Authority = issuer;
            oidcOptions.CallbackPath = new PathString(options.CallbackPath);
            oidcOptions.SignedOutCallbackPath = new PathString(OktaDefaults.SignOutCallbackPath);
            oidcOptions.ResponseType = OpenIdConnectResponseType.Code;
            oidcOptions.GetClaimsFromUserInfoEndpoint = options.GetClaimsFromUserInfoEndpoint;
            oidcOptions.SecurityTokenValidator = new StrictSecurityTokenValidator();
            oidcOptions.SaveTokens = true;
            oidcOptions.UseTokenLifetime = false;
            oidcOptions.BackchannelHttpHandler = new UserAgentHandler("okta-aspnetcore",
                typeof(OktaAuthenticationOptionsExtensions).Assembly.GetName().Version);

            var hasDefinedScopes = options.Scope?.Any() ?? false;
            if (hasDefinedScopes)
            {
                oidcOptions.Scope.Clear();
                foreach (var scope in options.Scope)
                {
                    oidcOptions.Scope.Add(scope);
                }
            }

            oidcOptions.TokenValidationParameters = new DefaultTokenValidationParameters(options, issuer)
            {
                ValidAudience = options.ClientId,
                NameClaimType = "name",
            };

            oidcOptions.Events.OnRedirectToIdentityProvider = events.OnRedirectToIdentityProvider;
        }
    }
}
