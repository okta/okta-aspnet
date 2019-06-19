// <copyright file="OpenIdConnectOptionsHelperShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using NSubstitute;
using Okta.AspNet.Abstractions;
using Xunit;

namespace Okta.AspNetCore.Test
{
    public class OpenIdConnectOptionsHelperShould
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SetOpenIdConnectsOptionsCorrectly(bool getClaimsFromUserInfoEndpoint)
        {
            var mockTokenValidatedEvent = Substitute.For<Func<TokenValidatedContext, Task>>();
            var mockUserInfoReceivedEvent = Substitute.For<Func<UserInformationReceivedContext, Task>>();

            var oktaMvcOptions = new OktaMvcOptions
            {
                PostLogoutRedirectUri = "http://foo.postlogout.com",
                AuthorizationServerId = "bar",
                ClientId = "foo",
                ClientSecret = "baz",
                OktaDomain = "http://myoktadomain.com",
                GetClaimsFromUserInfoEndpoint = getClaimsFromUserInfoEndpoint,
                CallbackPath = "/somecallbackpath",
                Scope = new List<string> { "openid", "profile", "email" },
                OnTokenValidated = mockTokenValidatedEvent,
                OnUserInformationReceived = mockUserInfoReceivedEvent,
            };

            var events = new OpenIdConnectEvents() { OnRedirectToIdentityProvider = null };

            var oidcOptions = new OpenIdConnectOptions();

            OpenIdConnectOptionsHelper.ConfigureOpenIdConnectOptions(oktaMvcOptions, events, oidcOptions);

            oidcOptions.ClientId.Should().Be(oktaMvcOptions.ClientId);
            oidcOptions.ClientSecret.Should().Be(oktaMvcOptions.ClientSecret);
            oidcOptions.SignedOutRedirectUri.Should().Be(oktaMvcOptions.PostLogoutRedirectUri);
            oidcOptions.GetClaimsFromUserInfoEndpoint.Should().Be(oktaMvcOptions.GetClaimsFromUserInfoEndpoint);
            oidcOptions.CallbackPath.Value.Should().Be(oktaMvcOptions.CallbackPath);

            var issuer = UrlHelper.CreateIssuerUrl(oktaMvcOptions.OktaDomain, oktaMvcOptions.AuthorizationServerId);
            oidcOptions.Authority.Should().Be(issuer);

            oidcOptions.Scope.ToList().Should().BeEquivalentTo(oktaMvcOptions.Scope);
            oidcOptions.CallbackPath.Value.Should().Be(oktaMvcOptions.CallbackPath);
            oidcOptions.Events.OnRedirectToIdentityProvider.Should().BeNull();

            // Check the event was call once with a null parameter
            oidcOptions.Events.OnTokenValidated(null);
            mockTokenValidatedEvent.Received(1).Invoke(null);

            // Check the event was call once with a null parameter
            oidcOptions.Events.OnUserInformationReceived(null);
            mockUserInfoReceivedEvent.Received(1).Invoke(null);
        }
    }
}
