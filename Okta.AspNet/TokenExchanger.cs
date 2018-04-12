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
            this._options = options;
            this._issuer = issuer;
            this._configurationManager = configurationManager;
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

            var userInfoClient = new UserInfoClient(openIdConfiguration.UserInfoEndpoint);
            var userInfoResponse = await userInfoClient.GetAsync(tokenResponse.AccessToken);

            var claims = new List<Claim>();
            claims.AddRange(userInfoResponse.Claims);
            claims.Add(new Claim("id_token", tokenResponse.IdentityToken));
            claims.Add(new Claim("access_token", tokenResponse.AccessToken));

            if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                claims.Add(new Claim("refresh_token", tokenResponse.RefreshToken));
            }

            response.AuthenticationTicket.Identity.AddClaims(claims);

            return;
        }
    }
}

