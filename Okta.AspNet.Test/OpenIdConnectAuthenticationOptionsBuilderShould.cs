// <copyright file="OpenIdConnectAuthenticationOptionsBuilderShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
                OpenIdConnectEvents = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = mockTokenEvent,
                },
                GetClaimsFromUserInfoEndpoint = false,
                BackchannelTimeout = TimeSpan.MaxValue,
                BackchannelHttpClientHandler = new MockHttpClientHandler(),
            };

            var oidcOptions = new OpenIdConnectAuthenticationOptionsBuilder("customAuthType", oktaMvcOptions).BuildOpenIdConnectAuthenticationOptions();

            oidcOptions.ClientId.Should().Be(oktaMvcOptions.ClientId);
            oidcOptions.ClientSecret.Should().Be(oktaMvcOptions.ClientSecret);
            oidcOptions.PostLogoutRedirectUri.Should().Be(oktaMvcOptions.PostLogoutRedirectUri);
            oidcOptions.AuthenticationMode.Should().Be(AuthenticationMode.Active);
            oidcOptions.BackchannelTimeout.Should().Be(TimeSpan.MaxValue);
            oidcOptions.BackchannelHttpHandler.GetType().Should().Be(typeof(MockHttpClientHandler));
            oidcOptions.AuthenticationType.Should().Be("customAuthType");

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

            var oidcOptions = new OpenIdConnectAuthenticationOptionsBuilder(OktaDefaults.MvcAuthenticationType, oktaMvcOptions, new MockUserInformationProvider(claims)).BuildOpenIdConnectAuthenticationOptions();

            oidcOptions.ClientId.Should().Be(oktaMvcOptions.ClientId);
            oidcOptions.ClientSecret.Should().Be(oktaMvcOptions.ClientSecret);
            oidcOptions.PostLogoutRedirectUri.Should().Be(oktaMvcOptions.PostLogoutRedirectUri);
            oidcOptions.AuthenticationMode.Should().Be(AuthenticationMode.Active);
            oidcOptions.AuthenticationType.Should().Be(OktaDefaults.MvcAuthenticationType);

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

            var oidcOptions = new OpenIdConnectAuthenticationOptionsBuilder(OktaDefaults.MvcAuthenticationType, oktaMvcOptions).BuildOpenIdConnectAuthenticationOptions();

            oidcOptions.AuthenticationMode.Should().Be(AuthenticationMode.Passive);
        }

        [Fact]
        public void SetBackchannelHttpHandlerWhenProxyConfigurationIsProvided()
        {
            var testProxy = "http://testproxy.cxm";
            var testPort = 8080;
            var oktaMvcOptions = new OktaMvcOptions()
            {
                PostLogoutRedirectUri = "http://postlogout.com",
                OktaDomain = "http://myoktadomain.com",
                ClientId = "foo",
                ClientSecret = "bar",
                RedirectUri = "/redirectUri",
                Scope = new List<string> { "openid", "profile", "email" },
                Proxy = new ProxyConfiguration
                {
                    Host = testProxy,
                    Port = testPort,
                },
            };

            var oidcOptions = new OpenIdConnectAuthenticationOptionsBuilder(OktaDefaults.MvcAuthenticationType, oktaMvcOptions).BuildOpenIdConnectAuthenticationOptions();
            oidcOptions.BackchannelHttpHandler.Should().BeOfType<OktaHttpMessageHandler>();
            var oktaHandler = (OktaHttpMessageHandler)oidcOptions.BackchannelHttpHandler;
            var proxy = ((HttpClientHandler)oktaHandler.InnerHandler).Proxy;
            proxy.Should().BeOfType<DefaultProxy>();
            proxy.GetProxy(new Uri("https://any.com")).ToString().Should().Be($"{testProxy}:{testPort}/");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ApplyUseTokenLifetimeCorrectly(bool useTokenLifetime)
        {
            var oktaMvcOptions = new OktaMvcOptions()
            {
                OktaDomain = "http://myoktadomain.com",
                ClientId = "foo",
                ClientSecret = "bar",
                UseTokenLifetime = useTokenLifetime,
            };

            var oidcOptions = new OpenIdConnectAuthenticationOptionsBuilder(
                OktaDefaults.MvcAuthenticationType,
                oktaMvcOptions).BuildOpenIdConnectAuthenticationOptions();

            oidcOptions.UseTokenLifetime.Should().Be(useTokenLifetime);
        }

        [Fact]
        public void DefaultUseTokenLifetimeToFalse()
        {
            var oktaMvcOptions = new OktaMvcOptions()
            {
                OktaDomain = "http://myoktadomain.com",
                ClientId = "foo",
                ClientSecret = "bar",
            };

            var oidcOptions = new OpenIdConnectAuthenticationOptionsBuilder(
                OktaDefaults.MvcAuthenticationType,
                oktaMvcOptions).BuildOpenIdConnectAuthenticationOptions();

            oidcOptions.UseTokenLifetime.Should().BeFalse();
        }
    }
}
