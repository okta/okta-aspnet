// <copyright file="UserInformationProvider.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Okta.AspNet
{
    public class UserInformationProvider : IUserInformationProvider
    {
        private readonly OktaMvcOptions _options;
        private readonly string _issuer;
        private readonly ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
        private static readonly HttpClient HttpClient = new HttpClient();

        public UserInformationProvider(OktaMvcOptions options, string issuer,
            ConfigurationManager<OpenIdConnectConfiguration> configurationManager)
        {
            _options = options;
            _issuer = issuer;
            _configurationManager = configurationManager;
        }

        public async Task EnrichIdentityViaUserInfoAsync(ClaimsIdentity subject, string accessToken)
        {
            // 1. Get the OIDC configuration to find the UserInfo endpoint
            var openIdConfiguration = await _configurationManager.GetConfigurationAsync().ConfigureAwait(false);
            var userInfoEndpoint = openIdConfiguration.UserInfoEndpoint;

            if (string.IsNullOrEmpty(userInfoEndpoint))
            {
                // Or handle this error as appropriate for your application
                throw new InvalidOperationException("UserInfo endpoint is not configured or could not be discovered.");
            }

            // 2. Make the request to the UserInfo endpoint using HttpClient
            using var request = new HttpRequestMessage(HttpMethod.Get, userInfoEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode(); // Throws if the response was not 2xx

            // 3. Parse the JSON response and create claims
            var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var userInfoData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonResponse);

            if (userInfoData == null)
            {
                return; // No claims to process
            }

            var userInfoClaims = new List<Claim>();
            foreach (var entry in userInfoData)
            {
                // The value of a claim can be a single value or an array.
                // This handles both cases by checking the JsonElement's value kind.
                if (entry.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in entry.Value.EnumerateArray())
                    {
                        userInfoClaims.Add(new Claim(entry.Key, item.ToString(), ClaimValueTypes.String, _issuer));
                    }
                }
                else
                {
                    userInfoClaims.Add(new Claim(entry.Key, entry.Value.ToString(), ClaimValueTypes.String, _issuer));
                }
            }

            // 4. Update the ClaimsIdentity with the new claims
            // Remove existing claims that will be replaced by the userinfo claims
            var duplicateClaims = subject.Claims
                .Where(c => userInfoClaims.Any(ui => ui.Type == c.Type))
                .ToList(); // ToList() is important to avoid modifying the collection while iterating

            foreach (var claim in duplicateClaims)
            {
                subject.RemoveClaim(claim);
            }

            subject.AddClaims(userInfoClaims);
        }
    }
}