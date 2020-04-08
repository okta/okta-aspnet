// <copyright file="UserInformationProvider.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Okta.AspNet
{
    public class UserInformationProvider : IUserInformationProvider
    {
        private readonly OktaMvcOptions _options;
        private readonly string _issuer;
        private readonly ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
        private static readonly HttpClient _httpClient = new HttpClient();

        public UserInformationProvider(OktaMvcOptions options, string issuer, ConfigurationManager<OpenIdConnectConfiguration> configurationManager)
        {
            _options = options;
            _issuer = issuer;
            _configurationManager = configurationManager;
        }

        public async Task EnrichIdentityViaUserInfoAsync(ClaimsIdentity subject, string accessToken)
        {
            var openIdConfiguration = await _configurationManager.GetConfigurationAsync().ConfigureAwait(false);

            var userInfoResponse = await _httpClient.GetUserInfoAsync(new UserInfoRequest
            {
                Address = openIdConfiguration.UserInfoEndpoint,
                Token = accessToken,
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
