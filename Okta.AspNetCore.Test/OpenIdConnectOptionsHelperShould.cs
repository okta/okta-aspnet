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

        #region EventsType DI Support Tests (Issue #247)

        /// <summary>
        /// Custom JwtBearerEvents class for testing EventsType DI support.
        /// </summary>
        public class CustomJwtBearerEvents : JwtBearerEvents
        {
            public bool WasConstructed { get; } = true;
        }

        /// <summary>
        /// Custom OpenIdConnectEvents class for testing EventsType DI support.
        /// </summary>
        public class CustomOpenIdConnectEvents : OpenIdConnectEvents
        {
            public bool WasConstructed { get; } = true;
        }

        [Fact]
        public void SetJwtBearerEventsType_WhenProvided()
        {
            // Arrange - Test for Issue #247
            // Verifies that JwtBearerEventsType is correctly mapped to JwtBearerOptions.EventsType
            var oktaWebApiOptions = new OktaWebApiOptions
            {
                AuthorizationServerId = "default",
                OktaDomain = "https://dev-12345.okta.com",
                Audience = "api://default",
                JwtBearerEventsType = typeof(CustomJwtBearerEvents),
            };

            var jwtBearerOptions = new JwtBearerOptions();

            // Act
            OpenIdConnectOptionsHelper.ConfigureJwtBearerOptions(oktaWebApiOptions, jwtBearerOptions);

            // Assert
            jwtBearerOptions.EventsType.Should().Be(typeof(CustomJwtBearerEvents),
                "EventsType should be set to enable DI resolution of custom events class");
        }

        [Fact]
        public void NotSetJwtBearerEventsType_WhenNotProvided()
        {
            // Arrange - Verify backward compatibility
            // When JwtBearerEventsType is not set, EventsType should remain null
            var oktaWebApiOptions = new OktaWebApiOptions
            {
                AuthorizationServerId = "default",
                OktaDomain = "https://dev-12345.okta.com",
                Audience = "api://default",
                // JwtBearerEventsType is NOT set
            };

            var jwtBearerOptions = new JwtBearerOptions();

            // Act
            OpenIdConnectOptionsHelper.ConfigureJwtBearerOptions(oktaWebApiOptions, jwtBearerOptions);

            // Assert
            jwtBearerOptions.EventsType.Should().BeNull(
                "EventsType should remain null when JwtBearerEventsType is not configured");
        }

        [Fact]
        public async Task SetBothJwtBearerEventsAndEventsType_EventsTypeWillTakePrecedence()
        {
            // Arrange - Test that EventsType takes precedence over Events instance
            // This is the expected ASP.NET Core behavior
            bool instanceEventInvoked = false;
            var oktaWebApiOptions = new OktaWebApiOptions
            {
                AuthorizationServerId = "default",
                OktaDomain = "https://dev-12345.okta.com",
                Audience = "api://default",
                JwtBearerEvents = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        instanceEventInvoked = true;
                        return Task.CompletedTask;
                    }
                },
                JwtBearerEventsType = typeof(CustomJwtBearerEvents),
            };

            var jwtBearerOptions = new JwtBearerOptions();

            // Act
            OpenIdConnectOptionsHelper.ConfigureJwtBearerOptions(oktaWebApiOptions, jwtBearerOptions);

            // Assert - Both should be set, but ASP.NET Core will prefer EventsType
            jwtBearerOptions.Events.Should().NotBeNull("Events instance should still be set");
            jwtBearerOptions.EventsType.Should().Be(typeof(CustomJwtBearerEvents),
                "EventsType should also be set and will take precedence at runtime");

            // The instance event can still be invoked (for testing), but in production ASP.NET Core
            // will resolve from DI when EventsType is set
            await jwtBearerOptions.Events.OnTokenValidated(default);
            instanceEventInvoked.Should().BeTrue("The instance events are still callable");
        }

        [Fact]
        public void SetOpenIdConnectEventsType_WhenProvided()
        {
            // Arrange - Test for Issue #247 (OpenIdConnect variant)
            // Verifies that OpenIdConnectEventsType is correctly mapped to OpenIdConnectOptions.EventsType
            var oktaMvcOptions = new OktaMvcOptions
            {
                AuthorizationServerId = "default",
                OktaDomain = "https://dev-12345.okta.com",
                ClientId = "test-client-id",
                ClientSecret = "test-client-secret",
                OpenIdConnectEventsType = typeof(CustomOpenIdConnectEvents),
            };

            var oidcOptions = new OpenIdConnectOptions();

            // Act
            OpenIdConnectOptionsHelper.ConfigureOpenIdConnectOptions(oktaMvcOptions, oidcOptions);

            // Assert
            oidcOptions.EventsType.Should().Be(typeof(CustomOpenIdConnectEvents),
                "EventsType should be set to enable DI resolution of custom events class");
        }

        [Fact]
        public void NotSetOpenIdConnectEventsType_WhenNotProvided()
        {
            // Arrange - Verify backward compatibility for OpenIdConnect
            // When OpenIdConnectEventsType is not set, EventsType should remain null
            var oktaMvcOptions = new OktaMvcOptions
            {
                AuthorizationServerId = "default",
                OktaDomain = "https://dev-12345.okta.com",
                ClientId = "test-client-id",
                ClientSecret = "test-client-secret",
                // OpenIdConnectEventsType is NOT set
            };

            var oidcOptions = new OpenIdConnectOptions();

            // Act
            OpenIdConnectOptionsHelper.ConfigureOpenIdConnectOptions(oktaMvcOptions, oidcOptions);

            // Assert
            oidcOptions.EventsType.Should().BeNull(
                "EventsType should remain null when OpenIdConnectEventsType is not configured");
        }

        [Fact]
        public async Task SetBothOpenIdConnectEventsAndEventsType_EventsTypeWillTakePrecedence()
        {
            // Arrange - Test that EventsType takes precedence over Events instance for OpenIdConnect
            bool instanceEventInvoked = false;
            var oktaMvcOptions = new OktaMvcOptions
            {
                AuthorizationServerId = "default",
                OktaDomain = "https://dev-12345.okta.com",
                ClientId = "test-client-id",
                ClientSecret = "test-client-secret",
                OpenIdConnectEvents = new OpenIdConnectEvents
                {
                    OnTokenValidated = context =>
                    {
                        instanceEventInvoked = true;
                        return Task.CompletedTask;
                    }
                },
                OpenIdConnectEventsType = typeof(CustomOpenIdConnectEvents),
            };

            var oidcOptions = new OpenIdConnectOptions();

            // Act
            OpenIdConnectOptionsHelper.ConfigureOpenIdConnectOptions(oktaMvcOptions, oidcOptions);

            // Assert - Both should be set, but ASP.NET Core will prefer EventsType
            oidcOptions.Events.Should().NotBeNull("Events instance should still be set");
            oidcOptions.EventsType.Should().Be(typeof(CustomOpenIdConnectEvents),
                "EventsType should also be set and will take precedence at runtime");

            // The instance event can still be invoked (for testing), but in production ASP.NET Core
            // will resolve from DI when EventsType is set
            await oidcOptions.Events.OnTokenValidated(default);
            instanceEventInvoked.Should().BeTrue("The instance events are still callable");
        }

        [Fact]
        public async Task ExistingJwtBearerEventsStillWork_WhenEventsTypeNotSet()
        {
            // Arrange - Verify no regression for existing functionality
            // Users who set JwtBearerEvents without EventsType should continue to work
            bool tokenValidatedInvoked = false;
            bool authFailedInvoked = false;

            var oktaWebApiOptions = new OktaWebApiOptions
            {
                AuthorizationServerId = "default",
                OktaDomain = "https://dev-12345.okta.com",
                Audience = "api://default",
                JwtBearerEvents = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        tokenValidatedInvoked = true;
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        authFailedInvoked = true;
                        return Task.CompletedTask;
                    }
                },
                // JwtBearerEventsType is NOT set - using instance-based events
            };

            var jwtBearerOptions = new JwtBearerOptions();

            // Act
            OpenIdConnectOptionsHelper.ConfigureJwtBearerOptions(oktaWebApiOptions, jwtBearerOptions);

            // Assert - Events should work exactly as before
            jwtBearerOptions.EventsType.Should().BeNull("EventsType should not be set");
            jwtBearerOptions.Events.Should().NotBeNull("Events instance should be set");

            await jwtBearerOptions.Events.OnTokenValidated(default);
            tokenValidatedInvoked.Should().BeTrue("OnTokenValidated event should be invoked");

            await jwtBearerOptions.Events.OnAuthenticationFailed(default);
            authFailedInvoked.Should().BeTrue("OnAuthenticationFailed event should be invoked");
        }

        [Fact]
        public async Task ExistingOpenIdConnectEventsStillWork_WhenEventsTypeNotSet()
        {
            // Arrange - Verify no regression for existing OpenIdConnect functionality
            bool tokenValidatedInvoked = false;
            bool remoteFailureInvoked = false;

            var oktaMvcOptions = new OktaMvcOptions
            {
                AuthorizationServerId = "default",
                OktaDomain = "https://dev-12345.okta.com",
                ClientId = "test-client-id",
                ClientSecret = "test-client-secret",
                OpenIdConnectEvents = new OpenIdConnectEvents
                {
                    OnTokenValidated = context =>
                    {
                        tokenValidatedInvoked = true;
                        return Task.CompletedTask;
                    },
                    OnRemoteFailure = context =>
                    {
                        remoteFailureInvoked = true;
                        return Task.CompletedTask;
                    }
                },
                // OpenIdConnectEventsType is NOT set - using instance-based events
            };

            var oidcOptions = new OpenIdConnectOptions();

            // Act
            OpenIdConnectOptionsHelper.ConfigureOpenIdConnectOptions(oktaMvcOptions, oidcOptions);

            // Assert - Events should work exactly as before
            oidcOptions.EventsType.Should().BeNull("EventsType should not be set");
            oidcOptions.Events.Should().NotBeNull("Events instance should be set");

            await oidcOptions.Events.OnTokenValidated(default);
            tokenValidatedInvoked.Should().BeTrue("OnTokenValidated event should be invoked");

            await oidcOptions.Events.OnRemoteFailure(default);
            remoteFailureInvoked.Should().BeTrue("OnRemoteFailure event should be invoked");
        }

        #endregion
    }
}
