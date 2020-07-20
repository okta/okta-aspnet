﻿// <copyright file="OktaAuthenticationOptionsExtensions.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Okta.AspNet.Abstractions;

namespace Okta.AspNetCore
{
    /// <summary>
    /// Okta OIDC extension methods.
    /// </summary>
    public static class OktaAuthenticationOptionsExtensions
    {
        /// <summary>
        /// Configures Okta for MVC applications.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <param name="options">The Okta MVC options.</param>
        /// <returns>The authentication builder.</returns>
        public static AuthenticationBuilder AddOktaMvc(this AuthenticationBuilder builder, OktaMvcOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            new OktaMvcOptionsValidator().Validate(options);

            return AddCodeFlow(builder, options);
        }

        /// <summary>
        /// Configures Okta for Web API apps.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <param name="options">The Okta Web API options.</param>
        /// <returns>The authentication builder.</returns>
        public static AuthenticationBuilder AddOktaWebApi(this AuthenticationBuilder builder, OktaWebApiOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            new OktaWebApiOptionsValidator().Validate(options);

            return AddJwtValidation(builder, options);
        }

        private static AuthenticationBuilder AddCodeFlow(AuthenticationBuilder builder, OktaMvcOptions options)
        {
            var events = new OpenIdConnectEvents
            {
                OnRedirectToIdentityProvider = BeforeRedirectToIdentityProviderAsync,
            };

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            builder.AddOpenIdConnect(oidcOptions => OpenIdConnectOptionsHelper.ConfigureOpenIdConnectOptions(options, events, oidcOptions));

            return builder;
        }

        private static Task BeforeRedirectToIdentityProviderAsync(RedirectContext context)
        {
            // Add sessionToken to provide custom login
            if (context.Properties.Items.TryGetValue("sessionToken", out var sessionToken))
            {
                if (!string.IsNullOrEmpty(sessionToken))
                {
                    context.ProtocolMessage.SetParameter("sessionToken", sessionToken);
                }
            }

            if (context.Properties.Items.TryGetValue("idp", out var idpId))
            {
                if (!string.IsNullOrEmpty(idpId))
                {
                    context.ProtocolMessage.SetParameter("idp", idpId);
                }
            }

            return Task.CompletedTask;
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
                opt.BackchannelHttpHandler = new OktaHttpMessageHandler("okta-aspnetcore", typeof(OktaAuthenticationOptionsExtensions).Assembly.GetName().Version, options);

                opt.SecurityTokenValidators.Clear();
                opt.SecurityTokenValidators.Add(new StrictSecurityTokenValidator());
            });

            return builder;
        }
    }
}
