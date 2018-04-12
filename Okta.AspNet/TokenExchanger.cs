using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security.Notifications;
using Okta.AspNet.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Okta.AspNet
{
    internal class TokenExchanger
    {
        private readonly OktaMvcOptions _options;
        private readonly string _issuer;
        private readonly ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;

        public TokenExchanger(OktaMvcOptions options, string issuer, ConfigurationManager<OpenIdConnectConfiguration> configurationManager)
        {
            _options = options;
            _issuer = issuer;
            _configurationManager = configurationManager;
        }

        public async Task ExchangeCodeForToken(AuthorizationCodeReceivedNotification response)
        {
            var openIdConfiguration = await _configurationManager.GetConfigurationAsync();
            var tokenClient = new TokenClient(openIdConfiguration.TokenEndpoint, _options.ClientId, _options.ClientSecret);
            var tokenResponse = await tokenClient.RequestAuthorizationCodeAsync(response.Code, _options.RedirectUri);

            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.Error);
            }

            if (_options.GetClaimsFromUserInfoEndpoint)
            {
                await EnrichIdentityViaUserInfo(
                    response.AuthenticationTicket.Identity,
                    openIdConfiguration,
                    tokenResponse);
            }

            response.AuthenticationTicket.Identity.AddClaim(new Claim("id_token", tokenResponse.IdentityToken));
            response.AuthenticationTicket.Identity.AddClaim(new Claim("access_token", tokenResponse.AccessToken));

            if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                response.AuthenticationTicket.Identity.AddClaim(new Claim("refresh_token", tokenResponse.RefreshToken));
            }

            return;
        }

        private async Task EnrichIdentityViaUserInfo(ClaimsIdentity subject, OpenIdConnectConfiguration openIdConfiguration, TokenResponse tokenResponse)
        {
            var userInfoClient = new UserInfoClient(openIdConfiguration.UserInfoEndpoint);
            var userInfoResponse = await userInfoClient.GetAsync(tokenResponse.AccessToken);

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

