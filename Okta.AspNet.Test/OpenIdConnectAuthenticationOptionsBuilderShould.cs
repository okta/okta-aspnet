// <copyright file="OpenIdConnectAuthenticationOptionsBuilderShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
                GetClaimsFromUserInfoEndpoint = false,
            };

            var oidcOptions = new OpenIdConnectAuthenticationOptionsBuilder(oktaMvcOptions).BuildOpenIdConnectAuthenticationOptions();

            oidcOptions.ClientId.Should().Be(oktaMvcOptions.ClientId);
            oidcOptions.ClientSecret.Should().Be(oktaMvcOptions.ClientSecret);
            oidcOptions.PostLogoutRedirectUri.Should().Be(oktaMvcOptions.PostLogoutRedirectUri);
            oidcOptions.AuthenticationMode.Should().Be(AuthenticationMode.Active);

            var issuer = UrlHelper.CreateIssuerUrl(oktaMvcOptions.OktaDomain, oktaMvcOptions.AuthorizationServerId);
            oidcOptions.Authority.Should().Be(issuer);
            oidcOptions.RedirectUri.Should().Be(oktaMvcOptions.RedirectUri);
            oidcOptions.Scope.Should().Be(string.Join(" ", oktaMvcOptions.Scope));

            SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context = new SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions>(null, null);
            context.AuthenticationTicket = new AuthenticationTicket(new ClaimsIdentity(), null);
            context.ProtocolMessage = new OpenIdConnectMessage() { AccessToken = "foo", IdToken = "bar" };
            // Check the event was call once with the corresponding context
            oidcOptions.Notifications.SecurityTokenValidated(context);
            mockTokenEvent.Received(1).Invoke(context);
        }

        [Fact]
        public void CallUserInformationProviderWhenGetClaimsFromUserInfoEndpointIsTrue()
        {
            var oktaMvcOptions = new OktaMvcOptions()
            {
                PostLogoutRedirectUri = "http://postlogout.com",
                OktaDomain = "http://myoktadomain.com",
                ClientId = "foo",
                ClientSecret = "bar",
                RedirectUri = "/redirectUri",
                Scope = new List<string> { "openid", "profile", "email" },
                GetClaimsFromUserInfoEndpoint = true,
            };

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("testClaimType", "testClaimValue"));

            var oidcOptions = new OpenIdConnectAuthenticationOptionsBuilder(oktaMvcOptions, new MockUserInformationProvider(claims)).BuildOpenIdConnectAuthenticationOptions();

            oidcOptions.ClientId.Should().Be(oktaMvcOptions.ClientId);
            oidcOptions.ClientSecret.Should().Be(oktaMvcOptions.ClientSecret);
            oidcOptions.PostLogoutRedirectUri.Should().Be(oktaMvcOptions.PostLogoutRedirectUri);
            oidcOptions.AuthenticationMode.Should().Be(AuthenticationMode.Active);

            var issuer = UrlHelper.CreateIssuerUrl(oktaMvcOptions.OktaDomain, oktaMvcOptions.AuthorizationServerId);
            oidcOptions.Authority.Should().Be(issuer);
            oidcOptions.RedirectUri.Should().Be(oktaMvcOptions.RedirectUri);
            oidcOptions.Scope.Should().Be(string.Join(" ", oktaMvcOptions.Scope));

            SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context = new SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions>(null, null);
            context.AuthenticationTicket = new AuthenticationTicket(new ClaimsIdentity(), null);
            context.ProtocolMessage = new OpenIdConnectMessage() { AccessToken = "foo", IdToken = "bar" };

            // This event should call UserInformationProvider.EnrichIdentityViaUserInfoAsync
            oidcOptions.Notifications.SecurityTokenValidated(context);

            context.AuthenticationTicket.Identity.Claims.Where(x => x.Type == "testClaimType").Count().Should().Be(1);
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

            var oidcOptions = new OpenIdConnectAuthenticationOptionsBuilder(oktaMvcOptions).BuildOpenIdConnectAuthenticationOptions();

            oidcOptions.AuthenticationMode.Should().Be(AuthenticationMode.Passive);
        }
    }
}
