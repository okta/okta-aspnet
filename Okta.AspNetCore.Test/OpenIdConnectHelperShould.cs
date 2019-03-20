// <copyright file="OpenIdConnectHelperShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Xunit;

namespace Okta.AspNetCore.Test
{
    public class OpenIdConnectHelperShould
    {
        [Fact]
        public void SetOpenIdConnectsOptionsCorrectly()
        {
            var oktaMvcOptions = new OktaMvcOptions
            {
                PostLogoutRedirectUri = "http://foo.postlogout.com",
                AuthorizationServerId = "bar",
                ClientId = "foo",
                ClientSecret = "baz",
                OktaDomain = "http://myoktadomain.com",
                GetClaimsFromUserInfoEndpoint = true,
                CallbackPath = "/somecallbackpath",
                Scope = new List<string> { "openid", "profile", "email" },
            };

            var events = new OpenIdConnectEvents() { OnRedirectToIdentityProvider = null };

            var oidcOptions = new OpenIdConnectOptions();

            OpenIdConnectOptionsHelper.ConfigureOpenIdConnectOptions(oktaMvcOptions, "issuer", events, oidcOptions);

            oidcOptions.ClientId.Should().Be(oktaMvcOptions.ClientId);
            oidcOptions.ClientSecret.Should().Be(oktaMvcOptions.ClientSecret);
            oidcOptions.SignedOutRedirectUri.Should().Be(oktaMvcOptions.PostLogoutRedirectUri);
            oidcOptions.GetClaimsFromUserInfoEndpoint.Should().Be(oktaMvcOptions.GetClaimsFromUserInfoEndpoint);
            oidcOptions.CallbackPath.Value.Should().Be(oktaMvcOptions.CallbackPath);
            oidcOptions.Authority.Should().Be("issuer");
            oidcOptions.Scope.ToList().Should().BeEquivalentTo(oktaMvcOptions.Scope);
            oidcOptions.CallbackPath.Value.Should().Be(oktaMvcOptions.CallbackPath);
            oidcOptions.Events.OnRedirectToIdentityProvider.Should().BeNull();
        }
    }
}
