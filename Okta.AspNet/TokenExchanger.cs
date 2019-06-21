// <copyright file="TokenExchanger.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security.Notifications;

namespace Okta.AspNet
{
    internal class TokenExchanger
    {
        private readonly OktaMvcOptions _options;
        private readonly string _issuer;
        private readonly ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
        private static readonly HttpClient _httpClient = new HttpClient();

        public TokenExchanger(OktaMvcOptions options, string issuer, ConfigurationManager<OpenIdConnectConfiguration> configurationManager)
        {
            _options = options;
            _issuer = issuer;
            _configurationManager = configurationManager;
        }

        public async Task ExchangeCodeForTokenAsync(AuthorizationCodeReceivedNotification response)
        {
            var openIdConfiguration = await _configurationManager.GetConfigurationAsync().ConfigureAwait(false);
            var tokenResponse = await _httpClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            {
                Address = openIdConfiguration.TokenEndpoint,
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret,
                Code = response.Code,
                RedirectUri = _options.RedirectUri,
            }).ConfigureAwait(false);

            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.Error);
            }

            FillNameIdentifierClaimOnIdentity(response.AuthenticationTicket.Identity);

            response.AuthenticationTicket.Identity.AddClaim(new Claim("id_token", tokenResponse.IdentityToken));
            response.AuthenticationTicket.Identity.AddClaim(new Claim("access_token", tokenResponse.AccessToken));

            if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                response.AuthenticationTicket.Identity.AddClaim(new Claim("refresh_token", tokenResponse.RefreshToken));
            }

            return;
        }

        /// <summary>
        /// For compatibility with the .NET MVC antiforgery provider, make sure the old-style NameIdentifier is filled.
        /// If not, get subject claim and duplicate it to MSFT's NameIdentifier.
        /// </summary>
        /// <param name="identity">The <see cref="ClaimsIdentity"/> to modify in place.</param>
        private void FillNameIdentifierClaimOnIdentity(ClaimsIdentity identity)
        {
            var currentNameIdentifier = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var sub = identity.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            if (currentNameIdentifier == null && sub != null)
            {
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, sub));
            }
        }
    }
}
