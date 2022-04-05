// <copyright file="OpenIdConnectOptionsHelper.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Okta.AspNet.Abstractions;

namespace Okta.AspNetCore
{
    /// <summary>
    /// Utility methods for OpenIdConnect options.
    /// </summary>
    public class OpenIdConnectOptionsHelper
    {
        /// <summary>
        /// Configure an OpenIdConnectOptions object based on user's configuration.
        /// </summary>
        /// <param name="oktaMvcOptions">The <see cref="OktaMvcOptions"/> options.</param>
        /// <param name="oidcOptions">The OpenIdConnectOptions to configure.</param>
        public static void ConfigureOpenIdConnectOptions(OktaMvcOptions oktaMvcOptions, OpenIdConnectOptions oidcOptions)
        {
            var issuer = UrlHelper.CreateIssuerUrl(oktaMvcOptions.OktaDomain, oktaMvcOptions.AuthorizationServerId);

            oidcOptions.ClientId = oktaMvcOptions.ClientId;
            oidcOptions.ClientSecret = oktaMvcOptions.ClientSecret;
            oidcOptions.Authority = issuer;
            oidcOptions.CallbackPath = new PathString(oktaMvcOptions.CallbackPath);
            oidcOptions.SignedOutCallbackPath = new PathString(OktaDefaults.SignOutCallbackPath);
            oidcOptions.SignedOutRedirectUri = oktaMvcOptions.PostLogoutRedirectUri;
            oidcOptions.ResponseType = OpenIdConnectResponseType.Code;
            oidcOptions.GetClaimsFromUserInfoEndpoint = oktaMvcOptions.GetClaimsFromUserInfoEndpoint;
            oidcOptions.SecurityTokenValidator = new StrictSecurityTokenValidator();
            oidcOptions.SaveTokens = true;
            oidcOptions.UseTokenLifetime = false;
            oidcOptions.BackchannelHttpHandler = new OktaHttpMessageHandler(
                "okta-aspnetcore",
                typeof(OktaAuthenticationOptionsExtensions).Assembly.GetName().Version,
                oktaMvcOptions);
            oidcOptions.BackchannelTimeout = oktaMvcOptions.BackchannelTimeout;

            var hasDefinedScopes = oktaMvcOptions.Scope?.Any() ?? false;
            if (hasDefinedScopes)
            {
                oidcOptions.Scope.Clear();
                foreach (var scope in oktaMvcOptions.Scope)
                {
                    oidcOptions.Scope.Add(scope);
                }
            }

            oidcOptions.TokenValidationParameters = new DefaultTokenValidationParameters(oktaMvcOptions, issuer)
            {
                ValidAudience = oktaMvcOptions.ClientId,
                NameClaimType = oktaMvcOptions.NameClaimType,
            };

            if (oktaMvcOptions.OpenIdConnectEvents != null)
            {
                oidcOptions.Events = oktaMvcOptions.OpenIdConnectEvents;
            }

            if (oktaMvcOptions.GetClaimsFromUserInfoEndpoint)
            {
                oidcOptions.ClaimActions.Add(new MapAllClaimsAction());
            }

            oidcOptions.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
            oidcOptions.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
            oidcOptions.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
            oidcOptions.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
            oidcOptions.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
        }

        /// <summary>
        /// Configure the JwtBearerOptions based on user's configuration.
        /// </summary>
        /// <param name="oktaWebApiOptions">The <see cref="OktaWebApiOptions"/> options.</param>
        /// <param name="jwtBearerOptions">The jwtBearerOptions to configure.</param>
        public static void ConfigureJwtBearerOptions(OktaWebApiOptions oktaWebApiOptions, JwtBearerOptions jwtBearerOptions)
        {
            var issuer = UrlHelper.CreateIssuerUrl(oktaWebApiOptions.OktaDomain, oktaWebApiOptions.AuthorizationServerId);

            var tokenValidationParameters = new DefaultTokenValidationParameters(oktaWebApiOptions, issuer)
            {
                ValidAudience = oktaWebApiOptions.Audience,
            };

            jwtBearerOptions.Audience = oktaWebApiOptions.Audience;
            jwtBearerOptions.Authority = issuer;
            jwtBearerOptions.TokenValidationParameters = tokenValidationParameters;
            jwtBearerOptions.BackchannelHttpHandler = new OktaHttpMessageHandler("okta-aspnetcore", typeof(OktaAuthenticationOptionsExtensions).Assembly.GetName().Version, oktaWebApiOptions);
            jwtBearerOptions.Events = oktaWebApiOptions.JwtBearerEvents ?? new JwtBearerEvents();
            jwtBearerOptions.SecurityTokenValidators.Clear();
            jwtBearerOptions.SecurityTokenValidators.Add(new StrictSecurityTokenValidator());
            jwtBearerOptions.BackchannelTimeout = oktaWebApiOptions.BackchannelTimeout;
        }
    }
}
