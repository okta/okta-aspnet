// <copyright file="OpenIdConnectAuthenticationOptionsBuilderShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using NSubstitute;
using Okta.AspNet.Abstractions;
using Xunit;

namespace Okta.AspNet.Test
{
    public class OpenIdConnectAuthenticationOptionsBuilderShould
    {
        [Fact]
        public void BuildOpenIdConnectAuthenticationOptionsCorrectly()
        {
            var mockTokenEvent = Substitute.For<Func<SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions>, Task>>();

            var oktaMvcOptions = new OktaMvcOptions()
            {
                PostLogoutRedirectUri = "http://postlogout.com",
                OktaDomain = "http://myoktadomain.com",
                ClientId = "foo",
                ClientSecret = "bar",
                RedirectUri = "/redirectUri",
                Scope = new List<string> { "openid", "profile", "email" },
                SecurityTokenValidated = mockTokenEvent,
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
            oidcOptions.AuthenticationMode.Should().Be(AuthenticationMode.Active);

            var issuer = UrlHelper.CreateIssuerUrl(oktaMvcOptions.OktaDomain, oktaMvcOptions.AuthorizationServerId);
            oidcOptions.Authority.Should().Be(issuer);
            oidcOptions.RedirectUri.Should().Be(oktaMvcOptions.RedirectUri);
            oidcOptions.Scope.Should().Be(string.Join(" ", oktaMvcOptions.Scope));

            // Check the event was call once with a null parameter
            oidcOptions.Notifications.SecurityTokenValidated(null);
            mockTokenEvent.Received(1).Invoke(null);
        }

        [Fact]
        public void SetAuthenticationModeToPassiveWhenLoginModeIsSelfHosted()
        {
            var oktaMvcOptions = new OktaMvcOptions()
            {
                PostLogoutRedirectUri = "http://postlogout.com",
                OktaDomain = "http://myoktadomain.com",
                ClientId = "foo",
                ClientSecret = "bar",
                RedirectUri = "/redirectUri",
                Scope = new List<string> { "openid", "profile", "email" },
                LoginMode = LoginMode.SelfHosted,
            };

            var notifications = new OpenIdConnectAuthenticationNotifications
            {
                RedirectToIdentityProvider = null,
            };

            var oidcOptions = OpenIdConnectAuthenticationOptionsBuilder.BuildOpenIdConnectAuthenticationOptions(
                oktaMvcOptions,
                notifications);

            oidcOptions.AuthenticationMode.Should().Be(AuthenticationMode.Passive);
        }
    }
}
