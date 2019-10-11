// <copyright file="OpenIdConnectAuthenticationOptionsBuilder.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Linq;
using System.Net.Http;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using Okta.AspNet.Abstractions;

namespace Okta.AspNet
{
    public class OpenIdConnectAuthenticationOptionsBuilder
    {
        /// <summary>
        /// Creates a new instance of OpenIdConnectAuthenticationOptions.
        /// </summary>
        /// <param name="oktaMvcOptions">The <see cref="OktaMvcOptions"/> options.</param>
        /// <param name="notifications">The OpenIdConnectAuthenticationNotifications notifications.</param>
        /// <returns>A new instance of OpenIdConnectAuthenticationOptions.</returns>
        public static OpenIdConnectAuthenticationOptions BuildOpenIdConnectAuthenticationOptions(OktaMvcOptions oktaMvcOptions, OpenIdConnectAuthenticationNotifications notifications)
        {
            var issuer = UrlHelper.CreateIssuerUrl(oktaMvcOptions.OktaDomain, oktaMvcOptions.AuthorizationServerId);
            var httpClient = new HttpClient(new UserAgentHandler("okta-aspnet", typeof(OktaMiddlewareExtensions).Assembly.GetName().Version));

            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                issuer + "/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever(httpClient));

            var tokenValidationParameters = new DefaultTokenValidationParameters(oktaMvcOptions, issuer)
            {
                NameClaimType = "name",
                ValidAudience = oktaMvcOptions.ClientId,
            };

            var tokenExchanger = new TokenExchanger(oktaMvcOptions, issuer, configurationManager);
            var definedScopes = oktaMvcOptions.Scope?.ToArray() ?? OktaDefaults.Scope;
            var scopeString = string.Join(" ", definedScopes);

            var oidcOptions = new OpenIdConnectAuthenticationOptions
            {
                AuthenticationType = oktaMvcOptions.AuthenticationType,
                ClientId = oktaMvcOptions.ClientId,
                ClientSecret = oktaMvcOptions.ClientSecret,
                Authority = issuer,
                RedirectUri = oktaMvcOptions.RedirectUri,
                ResponseType = OpenIdConnectResponseType.CodeIdToken,
                Scope = scopeString,
                PostLogoutRedirectUri = oktaMvcOptions.PostLogoutRedirectUri,
                TokenValidationParameters = tokenValidationParameters,
                SecurityTokenValidator = new StrictSecurityTokenValidator(),
                AuthenticationMode = (oktaMvcOptions.LoginMode == LoginMode.SelfHosted) ? AuthenticationMode.Passive : AuthenticationMode.Active,
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthorizationCodeReceived = tokenExchanger.ExchangeCodeForTokenAsync,
                    RedirectToIdentityProvider = notifications.RedirectToIdentityProvider,
                },
            };

            if (oktaMvcOptions.SecurityTokenValidated != null)
            {
                oidcOptions.Notifications.SecurityTokenValidated = oktaMvcOptions.SecurityTokenValidated;
            }

            return oidcOptions;
        }
    }
}
