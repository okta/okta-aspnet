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

            if (_options.GetClaimsFromUserInfoEndpoint)
            {
                await EnrichIdentityViaUserInfoAsync(
                    response.AuthenticationTicket.Identity,
                    openIdConfiguration,
                    tokenResponse).ConfigureAwait(false);
            }

            response.AuthenticationTicket.Identity.AddClaim(new Claim("id_token", tokenResponse.IdentityToken));
            response.AuthenticationTicket.Identity.AddClaim(new Claim("access_token", tokenResponse.AccessToken));

            if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                response.AuthenticationTicket.Identity.AddClaim(new Claim("refresh_token", tokenResponse.RefreshToken));
            }

            return;
        }

        private async Task EnrichIdentityViaUserInfoAsync(ClaimsIdentity subject, OpenIdConnectConfiguration openIdConfiguration, TokenResponse tokenResponse)
        {
            var userInfoResponse = await _httpClient.GetUserInfoAsync(new UserInfoRequest
            {
                Address = openIdConfiguration.UserInfoEndpoint,
                Token = tokenResponse.AccessToken,
            }).ConfigureAwait(false);

            // Claims returned from the UserInfoClient have issuer = "LOCAL AUTHORITY" by default
            var userInfoClaims = userInfoResponse.Claims
                .Select(x => new Claim(x.Type, x.Value, x.ValueType, _issuer, _issuer, subject))
                .ToArray();

            // Update ID token claims with fresh data from the /userinfo response
            var duplicateClaims = subject.Claims
                .Where(x => userInfoClaims.Any(y => y.Type == x.Type))
                .ToArray();

            foreach (var claim in duplicateClaims)
            {
                subject.RemoveClaim(claim);
            }

            subject.AddClaims(userInfoResponse.Claims);
        }
    }
}
