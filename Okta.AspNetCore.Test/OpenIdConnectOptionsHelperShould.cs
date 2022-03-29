// <copyright file="OpenIdConnectOptionsHelperShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using NSubstitute;
using Okta.AspNet.Abstractions;
using Xunit;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using JwtAuthenticationFailedContext = Microsoft.AspNetCore.Authentication.JwtBearer.AuthenticationFailedContext;
using OpenIdConnectTokenValidatedContext = Microsoft.AspNetCore.Authentication.OpenIdConnect.TokenValidatedContext;
using OpenIdConnectAuthenticationFailedContext = Microsoft.AspNetCore.Authentication.OpenIdConnect.AuthenticationFailedContext;
using JwtTokenValidatedContext = Microsoft.AspNetCore.Authentication.JwtBearer.TokenValidatedContext;

namespace Okta.AspNetCore.Test
{
    public class OpenIdConnectOptionsHelperShould
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SetOpenIdConnectsOptions(bool getClaimsFromUserInfoEndpoint)
        {
            var mockTokenValidatedEvent = Substitute.For<Func<OpenIdConnectTokenValidatedContext, Task>>();
            var mockUserInfoReceivedEvent = Substitute.For<Func<UserInformationReceivedContext, Task>>();
            var mockOktaExceptionEvent = Substitute.For<Func<RemoteFailureContext, Task>>();
            var mockAuthenticationFailedEvent = Substitute.For<Func<OpenIdConnectAuthenticationFailedContext, Task>>();
            var mockRedirectToIdentityProvider = Substitute.For<Func<RedirectContext, Task>>();
            var mockHttpHandler = Substitute.For<HttpMessageHandler>();

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
                BackchannelTimeout = TimeSpan.FromMinutes(5),
                BackchannelHttpClientHandler = mockHttpHandler,
                OpenIdConnectEvents = new OpenIdConnectEvents
                {
                    OnTokenValidated = mockTokenValidatedEvent,
                    OnUserInformationReceived = mockUserInfoReceivedEvent,
                    OnAuthenticationFailed = mockAuthenticationFailedEvent,
                    OnRemoteFailure = mockOktaExceptionEvent,
                    OnRedirectToIdentityProvider = mockRedirectToIdentityProvider
                }
            };

            var oidcOptions = new OpenIdConnectOptions();

            OpenIdConnectOptionsHelper.ConfigureOpenIdConnectOptions(oktaMvcOptions, oidcOptions);

            oidcOptions.ClientId.Should().Be(oktaMvcOptions.ClientId);
            oidcOptions.ClientSecret.Should().Be(oktaMvcOptions.ClientSecret);
            oidcOptions.SignedOutRedirectUri.Should().Be(oktaMvcOptions.PostLogoutRedirectUri);
            oidcOptions.GetClaimsFromUserInfoEndpoint.Should().Be(oktaMvcOptions.GetClaimsFromUserInfoEndpoint);
            oidcOptions.CallbackPath.Value.Should().Be(oktaMvcOptions.CallbackPath);
            oidcOptions.BackchannelTimeout.Should().Be(TimeSpan.FromMinutes(5));
            ((DelegatingHandler)oidcOptions.BackchannelHttpHandler).InnerHandler.Should().Be(mockHttpHandler);

            var jsonClaims = oidcOptions
                .ClaimActions.Where(ca => ca is JsonKeyClaimAction)
                .Cast<JsonKeyClaimAction>();
            jsonClaims.Should().Contain(ca => ca.ClaimType == ClaimTypes.Email && ca.JsonKey == "email");
            jsonClaims.Should().Contain(ca => ca.ClaimType == ClaimTypes.GivenName && ca.JsonKey == "given_name");
            jsonClaims.Should().Contain(ca => ca.ClaimType == ClaimTypes.Name && ca.JsonKey == "name");
            jsonClaims.Should().Contain(ca => ca.ClaimType == ClaimTypes.NameIdentifier && ca.JsonKey == "sub");
            jsonClaims.Should().Contain(ca => ca.ClaimType == ClaimTypes.Surname && ca.JsonKey == "family_name");

            var issuer = UrlHelper.CreateIssuerUrl(oktaMvcOptions.OktaDomain, oktaMvcOptions.AuthorizationServerId);
            oidcOptions.Authority.Should().Be(issuer);

            oidcOptions.Scope.ToList().Should().BeEquivalentTo(oktaMvcOptions.Scope);
            oidcOptions.CallbackPath.Value.Should().Be(oktaMvcOptions.CallbackPath);
            
            // Check the event was call once with a null parameter
            oidcOptions.Events.OnTokenValidated(null);
            mockTokenValidatedEvent.Received(1).Invoke(null);
            oidcOptions.Events.OnAuthenticationFailed(null);
            mockAuthenticationFailedEvent.Received(1).Invoke(null);
            oidcOptions.Events.OnRemoteFailure(null);
            mockOktaExceptionEvent.Received(1).Invoke(null);
            oidcOptions.Events.OnRedirectToIdentityProvider(null);
            mockRedirectToIdentityProvider.Received(1).Invoke(null);

            // UserInfo event is mapped only when GetClaimsFromUserInfoEndpoint = true
            if (oidcOptions.GetClaimsFromUserInfoEndpoint)
            {
                // Check the event was call once with a null parameter
                oidcOptions.Events.OnUserInformationReceived(null);
                mockUserInfoReceivedEvent.Received(1).Invoke(null);
            }
        }

        [Fact]
        public async Task SetJwtBearerOptions()
        {
            var mockHttpHandler = Substitute.For<HttpMessageHandler>();
            bool invoked = false;

            Func<JwtTokenValidatedContext, Task> mockTokenValidatedEvent = context =>
            {
                invoked = true;
                return Task.CompletedTask;
            };

            Func<JwtAuthenticationFailedContext, Task> mockAuthenticationFailedEvent = context =>
            {
                invoked = true;
                return Task.CompletedTask;
            };
            var oktaWebApiOptions = new OktaWebApiOptions
            {
                AuthorizationServerId = "bar",
                OktaDomain = "http://myoktadomain.com",
                Audience = "foo",
                BackchannelHttpClientHandler = mockHttpHandler,
                BackchannelTimeout = TimeSpan.FromMinutes(5),
                JwtBearerEvents = new JwtBearerEvents()
                {
                    OnTokenValidated = mockTokenValidatedEvent,
                    OnAuthenticationFailed = mockAuthenticationFailedEvent,
                }
            };
            
            var jwtBearerOptions = new JwtBearerOptions();

            OpenIdConnectOptionsHelper.ConfigureJwtBearerOptions(oktaWebApiOptions, jwtBearerOptions);
            var issuer = UrlHelper.CreateIssuerUrl(oktaWebApiOptions.OktaDomain, oktaWebApiOptions.AuthorizationServerId);
            jwtBearerOptions.Authority.Should().Be(issuer);
            jwtBearerOptions.Audience.Should().Be(oktaWebApiOptions.Audience);
            jwtBearerOptions.BackchannelTimeout.Should().Be(TimeSpan.FromMinutes(5));
            ((DelegatingHandler)jwtBearerOptions.BackchannelHttpHandler).InnerHandler.Should().Be(mockHttpHandler);

            await jwtBearerOptions.Events.OnTokenValidated(default);
            invoked.Should().BeTrue();
            invoked = false;
            await jwtBearerOptions.Events.OnAuthenticationFailed(default);
            invoked.Should().BeTrue();
        }
    }
}
