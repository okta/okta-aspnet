// <copyright file="OktaAuthenticationOptionsExtensions.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Okta.AspNet.Abstractions;

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
                options.SecurityTokenValidator = new StrictSecurityTokenHandler() { ClientId = oktaOptions.ClientId };
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
                        ClientId = oktaOptions.ClientId,
                    });
            });

            return builder;
        }
    }
}
