// <copyright file="JwtOptionsBuilder.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Okta.AspNet.Abstractions;

namespace Okta.AspNet
{
    public class JwtOptionsBuilder
    {
        /// <summary>
        /// Builds the JwtBearerAuthenticationOptions object used during the authentication process.
        /// </summary>
        /// <param name="authenticationType">The authentication type.</param>
        /// <param name="options">The Okta options.</param>
        /// <returns>An instance of JwtBearerAuthenticationOptions.</returns>
        public static JwtBearerAuthenticationOptions BuildJwtBearerAuthenticationOptions(string authenticationType, OktaWebApiOptions options)
        {
            var issuer = UrlHelper.CreateIssuerUrl(options.OktaDomain, options.AuthorizationServerId);

            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                issuer + "/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever(HttpClientBuilder.CreateClient(options)));

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

            return new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = tokenValidationParameters,
                TokenHandler = new StrictTokenHandler(),
                Provider = options.OAuthBearerAuthenticationProvider ?? new OAuthBearerAuthenticationProvider(),
                AuthenticationType = authenticationType,
            };
        }
    }
}
