// <copyright file="OktaAuthenticationOptionsExtensions.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
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
        public static AuthenticationBuilder AddOktaMvc(this AuthenticationBuilder builder, OktaMvcOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            new OktaMvcOptionsValidator().Validate(options);

            return AddCodeFlow(builder, options);
        }

        private static AuthenticationBuilder AddCodeFlow(AuthenticationBuilder builder, OktaMvcOptions options)
        {
            var issuer = UrlHelper.CreateIssuerUrl(options.OktaDomain, options.AuthorizationServerId);

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            builder.AddOpenIdConnect(oidcOptions =>
            {
                oidcOptions.ClientId = options.ClientId;
                oidcOptions.ClientSecret = options.ClientSecret;
                oidcOptions.Authority = issuer;
                oidcOptions.CallbackPath = new PathString(options.CallbackPath);
                oidcOptions.SignedOutCallbackPath = new PathString(OktaDefaults.SignOutCallbackPath);
                oidcOptions.ResponseType = OpenIdConnectResponseType.Code;
                oidcOptions.GetClaimsFromUserInfoEndpoint = options.GetClaimsFromUserInfoEndpoint;
                oidcOptions.SaveTokens = true;
                oidcOptions.UseTokenLifetime = false;
                oidcOptions.BackchannelHttpHandler = new UserAgentHandler();

                if (!string.IsNullOrEmpty(options.Scope))
                {
                    oidcOptions.Scope.Clear();
                    var scopes = options.Scope.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var scope in scopes)
                    {
                        oidcOptions.Scope.Add(scope);
                    }
                }

                oidcOptions.TokenValidationParameters = new DefaultTokenValidationParameters(options, issuer)
                {
                    ValidAudience = options.ClientId,
                    NameClaimType = "name",
                };
            });

            return builder;
        }

        public static AuthenticationBuilder AddOktaWebApi(this AuthenticationBuilder builder, OktaWebApiOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            new OktaWebApiOptionsValidator().Validate(options);

            return AddJwtValidation(builder, options);
        }

        private static AuthenticationBuilder AddJwtValidation(AuthenticationBuilder builder, OktaWebApiOptions options)
        {
            var issuer = UrlHelper.CreateIssuerUrl(options.OktaDomain, options.AuthorizationServerId);

            var tokenValidationParameters = new DefaultTokenValidationParameters(options, issuer)
            {
                ValidAudience = options.Audience,
            };

            builder.AddJwtBearer(opt =>
            {
                opt.Audience = options.Audience;
                opt.Authority = issuer;
                opt.TokenValidationParameters = tokenValidationParameters;
                opt.BackchannelHttpHandler = new UserAgentHandler();

                opt.SecurityTokenValidators.Clear();
                opt.SecurityTokenValidators.Add(new StrictSecurityTokenValidator(options));
            });

            return builder;
        }
    }
}
