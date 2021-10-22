// <copyright file="OktaMiddlewareExtensions.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Okta.AspNet.Abstractions;
using Owin;

namespace Okta.AspNet
{
    public static class OktaMiddlewareExtensions
    {
        public static IAppBuilder UseOktaMvc(this IAppBuilder app, OktaMvcOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            new OktaMvcOptionsValidator().Validate(options);
            AddOpenIdConnectAuthentication(app, options);

            return app;
        }

        public static IAppBuilder UseOktaMvcWithCookieManager(this IAppBuilder app, OktaMvcOptions options, ICookieManager cookieManager)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (cookieManager == null)
            {
                throw new ArgumentNullException(nameof(cookieManager));
            }

            new OktaMvcOptionsValidator().Validate(options);
            AddOpenIdConnectAuthentication(app, options, cookieManager);

            return app;
        }

        public static IAppBuilder UseOktaWebApi(this IAppBuilder app, OktaWebApiOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            new OktaWebApiOptionsValidator().Validate(options);
            AddJwtBearerAuthentication(app, options);

            return app;
        }

        private static void AddJwtBearerAuthentication(IAppBuilder app, OktaWebApiOptions options)
        {
            var issuer = UrlHelper.CreateIssuerUrl(options.OktaDomain, options.AuthorizationServerId);
            var httpClient = new HttpClient(new OktaHttpMessageHandler("okta-aspnet", typeof(OktaMiddlewareExtensions).Assembly.GetName().Version, options));

            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
              issuer + "/.well-known/openid-configuration",
              new OpenIdConnectConfigurationRetriever(),
              new HttpDocumentRetriever(httpClient));

            // Stop the default behavior of remapping JWT claim names to legacy MS/SOAP claim names
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var signingKeyCachingProvider = new DiscoveryDocumentCachingSigningKeyProvider(new DiscoveryDocumentSigningKeyProvider(configurationManager));

            var tokenValidationParameters = new DefaultTokenValidationParameters(options, issuer)
            {
                ValidAudience = options.Audience,
                IssuerSigningKeyResolver = (token, securityToken, keyId, validationParameters) =>
                {
                    return signingKeyCachingProvider.SigningKeys.Where(x => x.KeyId == keyId);
                },
            };

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = tokenValidationParameters,
                TokenHandler = new StrictTokenHandler(),
            });
        }

        private static void AddOpenIdConnectAuthentication(IAppBuilder app, OktaMvcOptions options, ICookieManager cookieManager = null)
        {
            // Stop the default behavior of remapping JWT claim names to legacy MS/SOAP claim names
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            if (cookieManager != null)
            {
                var opts = new OpenIdConnectAuthenticationOptionsBuilder(options).BuildOpenIdConnectAuthenticationOptions();
                opts.CookieManager = cookieManager;
                app.UseOpenIdConnectAuthentication(opts);
            }
            else
            {
                app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptionsBuilder(options).BuildOpenIdConnectAuthenticationOptions());
            }
        }
    }
}
