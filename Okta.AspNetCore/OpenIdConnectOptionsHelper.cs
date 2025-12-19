// <copyright file="OpenIdConnectOptionsHelper.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Okta.AspNet.Abstractions;

using JwtBearerAuthenticationFailedContext = Microsoft.AspNetCore.Authentication.JwtBearer.AuthenticationFailedContext;

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

#if NET8_0_OR_GREATER
            oidcOptions.TokenHandler = new JsonWebTokenHandler
            {
                MapInboundClaims = true,
            };
#else
            oidcOptions.SecurityTokenValidator = new StrictSecurityTokenValidator();
#endif
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
                NameClaimType = "name",
            };

            // Set up events with enhanced error handling for OIDC discovery failures
            var existingEvents = oktaMvcOptions.OpenIdConnectEvents ?? new OpenIdConnectEvents();
            var existingOnRemoteFailure = existingEvents.OnRemoteFailure;

            oidcOptions.Events = existingEvents;
            oidcOptions.Events.OnRemoteFailure = context => OnOpenIdConnectRemoteFailure(context, existingOnRemoteFailure, oktaMvcOptions);

            // Set EventsType for DI support - this takes precedence over the Events instance
            if (oktaMvcOptions.OpenIdConnectEventsType != null)
            {
                oidcOptions.EventsType = oktaMvcOptions.OpenIdConnectEventsType;
            }

#if NET9_0_OR_GREATER
            if (oktaMvcOptions.PushedAuthorizationBehavior.HasValue)
            {
                oidcOptions.PushedAuthorizationBehavior = oktaMvcOptions.PushedAuthorizationBehavior.Value;
            }
#endif

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
        /// Handles OpenIdConnect remote failures and wraps OIDC discovery errors with helpful messages.
        /// </summary>
        private static Task OnOpenIdConnectRemoteFailure(
            RemoteFailureContext context,
            Func<RemoteFailureContext, Task> existingHandler,
            OktaMvcOptions options)
        {
            // Check if this is an OIDC discovery failure that we should wrap
            if (context?.Failure != null && options?.OktaDomain != null && IsOidcDiscoveryFailure(context.Failure))
            {
                var enhancedException = OktaOidcException.CreateDiscoveryException(
                    context.Failure,
                    options.OktaDomain,
                    options.AuthorizationServerId);

                // Replace the failure with our enhanced one
                context.Failure = enhancedException;
            }

            // Call any existing handler
            if (existingHandler != null)
            {
                return existingHandler(context);
            }

            return Task.CompletedTask;
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
#if NET8_0_OR_GREATER
            jwtBearerOptions.TokenHandlers.Clear();
            jwtBearerOptions.TokenHandlers.Add(new JsonWebTokenHandler
            {
                MapInboundClaims = true, // Maintain backward compatibility by mapping JWT claim names to .NET ClaimTypes
            });

#else
            jwtBearerOptions.SecurityTokenValidators.Clear();
            jwtBearerOptions.SecurityTokenValidators.Add(new StrictSecurityTokenValidator());
#endif

            jwtBearerOptions.Audience = oktaWebApiOptions.Audience;
            jwtBearerOptions.Authority = issuer;
            jwtBearerOptions.TokenValidationParameters = tokenValidationParameters;
            jwtBearerOptions.BackchannelHttpHandler = new OktaHttpMessageHandler("okta-aspnetcore", typeof(OktaAuthenticationOptionsExtensions).Assembly.GetName().Version, oktaWebApiOptions);

            // Set up events with enhanced error handling for OIDC discovery failures
            var existingEvents = oktaWebApiOptions.JwtBearerEvents ?? new JwtBearerEvents();
            var existingOnAuthenticationFailed = existingEvents.OnAuthenticationFailed;

            jwtBearerOptions.Events = existingEvents;
            jwtBearerOptions.Events.OnAuthenticationFailed = context => OnJwtBearerAuthenticationFailed(context, existingOnAuthenticationFailed, oktaWebApiOptions);

            // Set EventsType for DI support - this takes precedence over the Events instance
            if (oktaWebApiOptions.JwtBearerEventsType != null)
            {
                jwtBearerOptions.EventsType = oktaWebApiOptions.JwtBearerEventsType;
            }

            jwtBearerOptions.BackchannelTimeout = oktaWebApiOptions.BackchannelTimeout;
        }

        /// <summary>
        /// Handles JWT Bearer authentication failures and wraps OIDC discovery errors with helpful messages.
        /// </summary>
        private static Task OnJwtBearerAuthenticationFailed(
            JwtBearerAuthenticationFailedContext context,
            Func<JwtBearerAuthenticationFailedContext, Task> existingHandler,
            OktaWebApiOptions options)
        {
            // Check if this is an OIDC discovery failure that we should wrap
            if (context?.Exception != null && options?.OktaDomain != null && IsOidcDiscoveryFailure(context.Exception))
            {
                var enhancedException = OktaOidcException.CreateDiscoveryException(
                    context.Exception,
                    options.OktaDomain,
                    options.AuthorizationServerId);

                // Replace the exception with our enhanced one
                context.Exception = enhancedException;
            }

            // Call any existing handler
            if (existingHandler != null)
            {
                return existingHandler(context);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Determines if an exception is related to OIDC discovery failure.
        /// </summary>
        private static bool IsOidcDiscoveryFailure(Exception exception)
        {
            if (exception == null)
            {
                return false;
            }

            // Check for common OIDC discovery failure patterns
            var message = exception.Message ?? string.Empty;

            // IDX20803: Unable to obtain configuration from: '...'
            // IDX20804: Unable to retrieve document from: '...'
            if (message.Contains("IDX20803") || message.Contains("IDX20804"))
            {
                return true;
            }

            // Check for HTTP errors that commonly indicate discovery failures
            if (exception is System.Net.Http.HttpRequestException)
            {
                return true;
            }

            // Check inner exception
            return IsOidcDiscoveryFailure(exception.InnerException);
        }
    }
}
