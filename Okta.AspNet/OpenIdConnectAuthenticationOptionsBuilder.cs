// <copyright file="OpenIdConnectAuthenticationOptionsBuilder.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Linq;
using System.Net.Http;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security.OpenIdConnect;
using Okta.AspNet.Abstractions;

namespace Okta.AspNet
{
    public class OpenIdConnectAuthenticationOptionsBuilder
    {
        public static OpenIdConnectAuthenticationOptions BuildOpenIdConnectAuthenticationOptions(OktaMvcOptions options, OpenIdConnectAuthenticationNotifications notifications)
        {
            var issuer = UrlHelper.CreateIssuerUrl(options.OktaDomain, options.AuthorizationServerId);
            var httpClient = new HttpClient(new UserAgentHandler("okta-aspnet", typeof(OktaMiddlewareExtensions).Assembly.GetName().Version));

            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                issuer + "/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever(httpClient));

            var tokenValidationParameters = new DefaultTokenValidationParameters(options, issuer)
            {
                NameClaimType = "name",
                ValidAudience = options.ClientId,
            };

            var tokenExchanger = new TokenExchanger(options, issuer, configurationManager);
            var definedScopes = options.Scope?.ToArray() ?? OktaDefaults.Scope;
            var scopeString = string.Join(" ", definedScopes);

            return new OpenIdConnectAuthenticationOptions
            {
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret,
                Authority = issuer,
                RedirectUri = options.RedirectUri,
                ResponseType = OpenIdConnectResponseType.CodeIdToken,
                Scope = scopeString,
                PostLogoutRedirectUri = options.PostLogoutRedirectUri,
                TokenValidationParameters = tokenValidationParameters,
                SecurityTokenValidator = new StrictSecurityTokenValidator(),
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthorizationCodeReceived = tokenExchanger.ExchangeCodeForTokenAsync,
                    RedirectToIdentityProvider = notifications.RedirectToIdentityProvider,
                },
            };
        }
    }
}
