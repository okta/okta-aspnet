// <copyright file="OpenIdConnectAuthenticationOptionsBuilderShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Owin.Security.OpenIdConnect;
using Okta.AspNet.Abstractions;
using Xunit;

namespace Okta.AspNet.Test
{
    public class OpenIdConnectAuthenticationOptionsBuilderShould
    {
        [Fact]
        public void BuildOpenIdConnectAuthenticationOptionsCorrectly()
        {
            var oktaMvcOptions = new OktaMvcOptions()
            {
                PostLogoutRedirectUri = "http://postlogout.com",
                OktaDomain = "http://myoktadomain.com",
                ClientId = "foo",
                ClientSecret = "bar",
                RedirectUri = "/redirectUri",
                Scope = new List<string> { "openid", "profile", "email" },
            };

            var notifications = new OpenIdConnectAuthenticationNotifications
            {
                RedirectToIdentityProvider = null,
            };

            var oidcOptions = OpenIdConnectAuthenticationOptionsBuilder.BuildOpenIdConnectAuthenticationOptions(
                oktaMvcOptions,
                notifications);

            oidcOptions.ClientId.Should().Be(oktaMvcOptions.ClientId);
            oidcOptions.ClientSecret.Should().Be(oktaMvcOptions.ClientSecret);
            oidcOptions.PostLogoutRedirectUri.Should().Be(oktaMvcOptions.PostLogoutRedirectUri);

            var issuer = UrlHelper.CreateIssuerUrl(oktaMvcOptions.OktaDomain, oktaMvcOptions.AuthorizationServerId);
            oidcOptions.Authority.Should().Be(issuer);
            oidcOptions.RedirectUri.Should().Be(oktaMvcOptions.RedirectUri);
            oidcOptions.Scope.Should().Be(string.Join(" ", oktaMvcOptions.Scope));
        }
    }
}
