using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Okta.AspNet.Abstractions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Okta.AspNetCore
{
    public static class OktaServiceCollectionExtensions
    {
        public static void AddOktaMvc(this IServiceCollection services, OktaMvcOptions oktaOptions)
        {
            var issuer = UrlHelper.CreateIssuerUrl(oktaOptions.OrgUrl, oktaOptions.AuthorizationServerId);

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options => 
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
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name"
                };
            });

        }

        public static void AddOktaWebApi(this IServiceCollection services, OktaWebApiOptions oktaOptions)
        {
            var issuer = UrlHelper.CreateIssuerUrl(oktaOptions.OrgUrl, oktaOptions.AuthorizationServerId);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = issuer;
                options.Audience = oktaOptions.Audience;
            });
        }
    }
}
