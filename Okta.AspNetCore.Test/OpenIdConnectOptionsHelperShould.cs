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
        public async Task SetOpenIdConnectsOptions(bool getClaimsFromUserInfoEndpoint)
        {
            bool invoked = false;

            Func<OpenIdConnectTokenValidatedContext, Task> mockTokenValidatedEvent = context =>
            {
                invoked = true;
                return Task.CompletedTask;
            };

            Func<UserInformationReceivedContext, Task> mockUserInfoReceivedEvent = context =>
            {
                invoked = true;
                return Task.CompletedTask;
            };

            Func<RemoteFailureContext, Task> mockOktaExceptionEvent = context =>
            {
                invoked = true;
                return Task.CompletedTask;
            };

            Func<OpenIdConnectAuthenticationFailedContext, Task> mockAuthenticationFailedEvent = context =>
            {
                invoked = true;
                return Task.CompletedTask;
            };

            Func<RedirectContext, Task> mockRedirectToIdentityProvider = context =>
            {
                invoked = true;
                return Task.CompletedTask;
            };

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
            await oidcOptions.Events.OnTokenValidated(default);
            invoked.Should().BeTrue();
            invoked = false;
            
            await oidcOptions.Events.OnAuthenticationFailed(default);
            invoked.Should().BeTrue();
            invoked = false;

            
            await oidcOptions.Events.OnRemoteFailure(default);
            invoked.Should().BeTrue();
            invoked = false;

            
            await oidcOptions.Events.OnRedirectToIdentityProvider(default);
            invoked.Should().BeTrue();
            invoked = false;

            // UserInfo event is mapped only when GetClaimsFromUserInfoEndpoint = true
            if (oidcOptions.GetClaimsFromUserInfoEndpoint)
            {
                // Check the event was call once with a null parameter
                await oidcOptions.Events.OnUserInformationReceived(default);
                invoked.Should().BeTrue();
                invoked = false;
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

#if NET8_0_OR_GREATER
        [Fact]
        public void ConfigureJsonWebTokenHandlerWithMapInboundClaimsEnabled()
        {
            // Arrange - This test verifies the fix for Issue #286
            // https://github.com/okta/okta-aspnet/issues/286
            var oktaMvcOptions = new OktaMvcOptions
            {
                OktaDomain = "https://dev-12345.okta.com",
                ClientId = "foo",
                ClientSecret = "bar",
            };

            var oidcOptions = new OpenIdConnectOptions();

            // Act
            OpenIdConnectOptionsHelper.ConfigureOpenIdConnectOptions(oktaMvcOptions, oidcOptions);

            // Assert - Verify JsonWebTokenHandler is configured with MapInboundClaims = true
            oidcOptions.TokenHandler.Should().NotBeNull("TokenHandler should be set for .NET 8+");
            
            var handler = oidcOptions.TokenHandler as Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler;
            handler.Should().NotBeNull("TokenHandler should be a JsonWebTokenHandler");
            handler.MapInboundClaims.Should().BeTrue(
                "MapInboundClaims must be true to maintain backward compatibility and map JWT claims like 'sub' to ClaimTypes.NameIdentifier");
        }

        [Fact]
        public void ConfigureJwtBearerWithJsonWebTokenHandlerMapInboundClaimsEnabled()
        {
            // Arrange - This test verifies the fix for Issue #286 for Web API scenarios
            var oktaWebApiOptions = new OktaWebApiOptions()
            {
                OktaDomain = "https://dev-12345.okta.com",
                AuthorizationServerId = "default",
                Audience = "api://default",
            };

            var jwtBearerOptions = new JwtBearerOptions();

            // Act
            OpenIdConnectOptionsHelper.ConfigureJwtBearerOptions(oktaWebApiOptions, jwtBearerOptions);

            // Assert - Verify JsonWebTokenHandler is configured with MapInboundClaims = true
            jwtBearerOptions.TokenHandlers.Should().NotBeNullOrEmpty("TokenHandlers should be configured for .NET 8+");
            jwtBearerOptions.TokenHandlers.Should().HaveCount(1, "Should have exactly one token handler");
            
            var handler = jwtBearerOptions.TokenHandlers.First() as Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler;
            handler.Should().NotBeNull("First token handler should be a JsonWebTokenHandler");
            handler.MapInboundClaims.Should().BeTrue(
                "MapInboundClaims must be true to maintain backward compatibility and map JWT claims like 'sub' to ClaimTypes.NameIdentifier");
        }

        [Fact]
        public async Task MapJwtClaimsToStandardClaimTypesForMvc()
        {
            // Arrange - This is an end-to-end test for Issue #286
            // Simulates actual JWT token validation and verifies claims are mapped correctly
            var oktaMvcOptions = new OktaMvcOptions
            {
                OktaDomain = "https://dev-12345.okta.com",
                ClientId = "test-client-id",
                ClientSecret = "test-secret",
            };

            var oidcOptions = new OpenIdConnectOptions();
            OpenIdConnectOptionsHelper.ConfigureOpenIdConnectOptions(oktaMvcOptions, oidcOptions);

            // Create a sample JWT token with standard Okta claims
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(),
                Claims = new Dictionary<string, object>
                {
                    { "sub", "00u1234567890abcdef" },
                    { "email", "test@example.com" },
                    { "given_name", "Test" },
                    { "family_name", "User" },
                    { "name", "Test User" },
                },
            };
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Act - Use the configured TokenHandler to validate the token
            var jsonWebTokenHandler = oidcOptions.TokenHandler as Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler;
            var validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = false,
                SignatureValidator = (token, parameters) => new Microsoft.IdentityModel.JsonWebTokens.JsonWebToken(token),
            };

            var result = await jsonWebTokenHandler.ValidateTokenAsync(tokenString, validationParameters);
            var identity = new ClaimsIdentity(result.ClaimsIdentity);

            // Assert - Verify that JWT claims are mapped to standard .NET ClaimTypes
            // This is the core of Issue #286 - these lookups should succeed
            var nameIdentifierClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            nameIdentifierClaim.Should().NotBeNull("sub should be mapped to ClaimTypes.NameIdentifier");
            nameIdentifierClaim.Value.Should().Be("00u1234567890abcdef");

            var emailClaim = identity.FindFirst(ClaimTypes.Email);
            emailClaim.Should().NotBeNull("email should be mapped to ClaimTypes.Email");
            emailClaim.Value.Should().Be("test@example.com");

            var givenNameClaim = identity.FindFirst(ClaimTypes.GivenName);
            givenNameClaim.Should().NotBeNull("given_name should be mapped to ClaimTypes.GivenName");
            givenNameClaim.Value.Should().Be("Test");

            var surnameClaim = identity.FindFirst(ClaimTypes.Surname);
            surnameClaim.Should().NotBeNull("family_name should be mapped to ClaimTypes.Surname");
            surnameClaim.Value.Should().Be("User");

            // Verify the actual claim types are the full URIs, not short names
            nameIdentifierClaim.Type.Should().Be("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            emailClaim.Type.Should().Be("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
        }
#endif
    }
}
