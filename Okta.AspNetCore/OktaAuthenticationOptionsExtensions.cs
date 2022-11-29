﻿// <copyright file="OktaAuthenticationOptionsExtensions.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        /// Configures Okta for MVC applications using the default authentication scheme <see cref="OpenIdConnectDefaults.AuthenticationScheme"/>.
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

            return AddCodeFlow(builder, OpenIdConnectDefaults.AuthenticationScheme, options);
        }

        /// <summary>
        /// Configures Okta for MVC applications using the specified authentication scheme.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="options">The Okta MVC options.</param>
        /// <returns>The authentication builder.</returns>
        public static AuthenticationBuilder AddOktaMvc(this AuthenticationBuilder builder, string authenticationScheme, OktaMvcOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (authenticationScheme == null)
            {
                throw new ArgumentNullException(nameof(authenticationScheme));
            }

            new OktaMvcOptionsValidator().Validate(options);

            return AddCodeFlow(builder, authenticationScheme, options);
        }

        /// <summary>
        /// Configures Okta for Web API apps using the default authentication scheme <see cref="JwtBearerDefaults.AuthenticationScheme"/>.
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

            return AddJwtValidation(builder, JwtBearerDefaults.AuthenticationScheme, options);
        }

        /// <summary>
        /// Configures Okta for Web API apps using the specified authentication scheme.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="options">The Okta Web API options.</param>
        /// <returns>The authentication builder.</returns>
        public static AuthenticationBuilder AddOktaWebApi(this AuthenticationBuilder builder, string authenticationScheme, OktaWebApiOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (authenticationScheme == null)
            {
                throw new ArgumentNullException(nameof(authenticationScheme));
            }

            new OktaWebApiOptionsValidator().Validate(options);

            return AddJwtValidation(builder, authenticationScheme, options);
        }

        private static AuthenticationBuilder AddCodeFlow(AuthenticationBuilder builder, string authenticationScheme, OktaMvcOptions options)
        {
            Func<RedirectContext, Task> redirectEvent = options.OpenIdConnectEvents?.OnRedirectToIdentityProvider;
            options.OpenIdConnectEvents ??= new OpenIdConnectEvents();
            options.OpenIdConnectEvents.OnRedirectToIdentityProvider = context => BeforeRedirectToIdentityProviderAsync(context, redirectEvent);

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            builder.AddOpenIdConnect(authenticationScheme, oidcOptions => OpenIdConnectOptionsHelper.ConfigureOpenIdConnectOptions(options, oidcOptions));

            return builder;
        }

        private static Task BeforeRedirectToIdentityProviderAsync(RedirectContext context, Func<RedirectContext, Task> redirectEvent)
        {
            // Verify if additional well-known params (e.g login-hint, sessionToken, idp, etc.) should be sent in the request.
            var oktaRequestParamValue = string.Empty;

            foreach (var oktaParamKey in OktaParams.AllParams)
            {
                context.Properties?.Items?.TryGetValue(oktaParamKey, out oktaRequestParamValue);

                if (!string.IsNullOrEmpty(oktaRequestParamValue))
                {
                    context.ProtocolMessage.SetParameter(oktaParamKey, oktaRequestParamValue);
                }
            }

            if (OktaParams.IsPromptEnrollAuthenticator(context.ProtocolMessage.Parameters))
            {
                // ACR values should be provided by the user.
                // scope, nonce, and resource must be omitted by the server.
                context.ProtocolMessage.ResponseType = "none";
                context.ProtocolMessage.MaxAge = "0";
                context.ProtocolMessage.Nonce = null;
                context.ProtocolMessage.Scope = null;
                context.ProtocolMessage.Resource = null;
                context.ProtocolMessage.RedirectUri = context?.Properties?.RedirectUri ?? context.ProtocolMessage.RedirectUri;

            }

            if (redirectEvent != null)
            {
                return redirectEvent(context);
            }

            return Task.CompletedTask;
        }

        private static AuthenticationBuilder AddJwtValidation(AuthenticationBuilder builder, string authenticationScheme, OktaWebApiOptions options) => builder.AddJwtBearer(authenticationScheme, opt => OpenIdConnectOptionsHelper.ConfigureJwtBearerOptions(options, opt));
    }
}
