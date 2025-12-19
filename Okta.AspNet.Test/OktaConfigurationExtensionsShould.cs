// <copyright file="OktaConfigurationExtensionsShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Okta.AspNet.Test
{
    public class OktaConfigurationExtensionsShould
    {
        private const string ValidOktaDomain = "https://myOktaDomain.oktapreview.com";

        [Fact]
        public void BindOktaDomainForWebApi()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:OktaDomain", ValidOktaDomain },
            });

            var options = configuration.GetOktaWebApiOptions();

            options.OktaDomain.Should().Be(ValidOktaDomain);
        }

        [Fact]
        public void BindAuthorizationServerIdForWebApi()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:OktaDomain", ValidOktaDomain },
                { "Okta:AuthorizationServerId", "custom" },
            });

            var options = configuration.GetOktaWebApiOptions();

            options.AuthorizationServerId.Should().Be("custom");
        }

        [Fact]
        public void BindAudienceForWebApi()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:OktaDomain", ValidOktaDomain },
                { "Okta:Audience", "api://myapi" },
            });

            var options = configuration.GetOktaWebApiOptions();

            options.Audience.Should().Be("api://myapi");
        }

        [Fact]
        public void UseDefaultAudienceWhenNotSpecified()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:OktaDomain", ValidOktaDomain },
            });

            var options = configuration.GetOktaWebApiOptions();

            options.Audience.Should().Be(Abstractions.OktaWebApiOptions.DefaultAudience);
        }

        [Fact]
        public void UseDefaultAuthorizationServerIdWhenNotSpecified()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:OktaDomain", ValidOktaDomain },
            });

            var options = configuration.GetOktaWebApiOptions();

            options.AuthorizationServerId.Should().Be(Abstractions.OktaWebOptions.DefaultAuthorizationServerId);
        }

        [Fact]
        public void BindClockSkewAsSecondsForWebApi()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:OktaDomain", ValidOktaDomain },
                { "Okta:ClockSkew", "300" },
            });

            var options = configuration.GetOktaWebApiOptions();

            options.ClockSkew.Should().Be(TimeSpan.FromSeconds(300));
        }

        [Fact]
        public void BindClockSkewAsTimeSpanForWebApi()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:OktaDomain", ValidOktaDomain },
                { "Okta:ClockSkew", "00:10:00" },
            });

            var options = configuration.GetOktaWebApiOptions();

            options.ClockSkew.Should().Be(TimeSpan.FromMinutes(10));
        }

        [Fact]
        public void BindBackchannelTimeoutAsSecondsForWebApi()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:OktaDomain", ValidOktaDomain },
                { "Okta:BackchannelTimeout", "60" },
            });

            var options = configuration.GetOktaWebApiOptions();

            options.BackchannelTimeout.Should().Be(TimeSpan.FromSeconds(60));
        }

        [Fact]
        public void BindBackchannelTimeoutAsTimeSpanForWebApi()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:OktaDomain", ValidOktaDomain },
                { "Okta:BackchannelTimeout", "00:02:00" },
            });

            var options = configuration.GetOktaWebApiOptions();

            options.BackchannelTimeout.Should().Be(TimeSpan.FromMinutes(2));
        }

        [Fact]
        public void BindOktaDomainForMvc()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:OktaDomain", ValidOktaDomain },
            });

            var options = configuration.GetOktaMvcOptions();

            options.OktaDomain.Should().Be(ValidOktaDomain);
        }

        [Fact]
        public void BindClientIdForMvc()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:OktaDomain", ValidOktaDomain },
                { "Okta:ClientId", "my-client-id" },
            });

            var options = configuration.GetOktaMvcOptions();

            options.ClientId.Should().Be("my-client-id");
        }

        [Fact]
        public void BindClientSecretForMvc()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:OktaDomain", ValidOktaDomain },
                { "Okta:ClientSecret", "my-client-secret" },
            });

            var options = configuration.GetOktaMvcOptions();

            options.ClientSecret.Should().Be("my-client-secret");
        }

        [Fact]
        public void BindRedirectUriForMvc()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:OktaDomain", ValidOktaDomain },
                { "Okta:RedirectUri", "https://localhost:5001/callback" },
            });

            var options = configuration.GetOktaMvcOptions();

            options.RedirectUri.Should().Be("https://localhost:5001/callback");
        }

        [Fact]
        public void BindPostLogoutRedirectUriForMvc()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:OktaDomain", ValidOktaDomain },
                { "Okta:PostLogoutRedirectUri", "https://localhost:5001/signout" },
            });

            var options = configuration.GetOktaMvcOptions();

            options.PostLogoutRedirectUri.Should().Be("https://localhost:5001/signout");
        }

        [Fact]
        public void BindGetClaimsFromUserInfoEndpointForMvc()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:OktaDomain", ValidOktaDomain },
                { "Okta:GetClaimsFromUserInfoEndpoint", "false" },
            });

            var options = configuration.GetOktaMvcOptions();

            options.GetClaimsFromUserInfoEndpoint.Should().BeFalse();
        }

        [Fact]
        public void BindUseTokenLifetimeForMvc()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:OktaDomain", ValidOktaDomain },
                { "Okta:UseTokenLifetime", "true" },
            });

            var options = configuration.GetOktaMvcOptions();

            options.UseTokenLifetime.Should().BeTrue();
        }

        [Fact]
        public void BindLoginModeForMvc()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:OktaDomain", ValidOktaDomain },
                { "Okta:LoginMode", "SelfHosted" },
            });

            var options = configuration.GetOktaMvcOptions();

            options.LoginMode.Should().Be(LoginMode.SelfHosted);
        }

        [Fact]
        public void BindLoginModeOktaHostedForMvc()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:OktaDomain", ValidOktaDomain },
                { "Okta:LoginMode", "OktaHosted" },
            });

            var options = configuration.GetOktaMvcOptions();

            options.LoginMode.Should().Be(LoginMode.OktaHosted);
        }

        [Fact]
        public void BindScopesForMvc()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:OktaDomain", ValidOktaDomain },
                { "Okta:Scope:0", "openid" },
                { "Okta:Scope:1", "profile" },
                { "Okta:Scope:2", "email" },
                { "Okta:Scope:3", "custom_scope" },
            });

            var options = configuration.GetOktaMvcOptions();

            options.Scope.Should().BeEquivalentTo(new[] { "openid", "profile", "email", "custom_scope" });
        }

        [Fact]
        public void UseDefaultScopesWhenNotSpecified()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:OktaDomain", ValidOktaDomain },
            });

            var options = configuration.GetOktaMvcOptions();

            options.Scope.Should().BeEquivalentTo(OktaDefaults.Scope);
        }

        [Fact]
        public void BindFromCustomSectionNameForWebApi()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "MyAuth:OktaDomain", ValidOktaDomain },
                { "MyAuth:AuthorizationServerId", "custom-server" },
            });

            var options = configuration.GetOktaWebApiOptions("MyAuth");

            options.OktaDomain.Should().Be(ValidOktaDomain);
            options.AuthorizationServerId.Should().Be("custom-server");
        }

        [Fact]
        public void BindFromCustomSectionNameForMvc()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "MyAuth:OktaDomain", ValidOktaDomain },
                { "MyAuth:ClientId", "custom-client" },
                { "MyAuth:ClientSecret", "custom-secret" },
            });

            var options = configuration.GetOktaMvcOptions("MyAuth");

            options.OktaDomain.Should().Be(ValidOktaDomain);
            options.ClientId.Should().Be("custom-client");
            options.ClientSecret.Should().Be("custom-secret");
        }

        [Fact]
        public void BindAllWebApiOptions()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:OktaDomain", ValidOktaDomain },
                { "Okta:AuthorizationServerId", "custom" },
                { "Okta:Audience", "api://myapi" },
                { "Okta:ClockSkew", "300" },
                { "Okta:BackchannelTimeout", "60" },
            });

            var options = configuration.GetOktaWebApiOptions();

            options.OktaDomain.Should().Be(ValidOktaDomain);
            options.AuthorizationServerId.Should().Be("custom");
            options.Audience.Should().Be("api://myapi");
            options.ClockSkew.Should().Be(TimeSpan.FromSeconds(300));
            options.BackchannelTimeout.Should().Be(TimeSpan.FromSeconds(60));
        }

        [Fact]
        public void BindAllMvcOptions()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:OktaDomain", ValidOktaDomain },
                { "Okta:ClientId", "my-client-id" },
                { "Okta:ClientSecret", "my-client-secret" },
                { "Okta:AuthorizationServerId", "custom" },
                { "Okta:RedirectUri", "https://localhost:5001/callback" },
                { "Okta:PostLogoutRedirectUri", "https://localhost:5001/signout" },
                { "Okta:GetClaimsFromUserInfoEndpoint", "false" },
                { "Okta:UseTokenLifetime", "true" },
                { "Okta:LoginMode", "SelfHosted" },
                { "Okta:ClockSkew", "300" },
                { "Okta:Scope:0", "openid" },
                { "Okta:Scope:1", "profile" },
            });

            var options = configuration.GetOktaMvcOptions();

            options.OktaDomain.Should().Be(ValidOktaDomain);
            options.ClientId.Should().Be("my-client-id");
            options.ClientSecret.Should().Be("my-client-secret");
            options.AuthorizationServerId.Should().Be("custom");
            options.RedirectUri.Should().Be("https://localhost:5001/callback");
            options.PostLogoutRedirectUri.Should().Be("https://localhost:5001/signout");
            options.GetClaimsFromUserInfoEndpoint.Should().BeFalse();
            options.UseTokenLifetime.Should().BeTrue();
            options.LoginMode.Should().Be(LoginMode.SelfHosted);
            options.ClockSkew.Should().Be(TimeSpan.FromSeconds(300));
            options.Scope.Should().BeEquivalentTo(new[] { "openid", "profile" });
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenConfigurationIsNullForWebApi()
        {
            IConfiguration configuration = null;

            Action act = () => configuration.GetOktaWebApiOptions();

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("configuration");
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenConfigurationIsNullForMvc()
        {
            IConfiguration configuration = null;

            Action act = () => configuration.GetOktaMvcOptions();

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("configuration");
        }

        [Fact]
        public void ReturnEmptyOptionsWhenSectionDoesNotExist()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "SomeOther:Setting", "value" },
            });

            var options = configuration.GetOktaWebApiOptions();

            options.OktaDomain.Should().BeNull();
            options.AuthorizationServerId.Should().Be(Abstractions.OktaWebOptions.DefaultAuthorizationServerId);
            options.Audience.Should().Be(Abstractions.OktaWebApiOptions.DefaultAudience);
        }

        [Fact]
        public void BindFromJsonConfiguration()
        {
            var json = @"{
                ""Okta"": {
                    ""OktaDomain"": ""https://test.okta.com"",
                    ""AuthorizationServerId"": ""default"",
                    ""Audience"": ""api://test"",
                    ""ClockSkew"": 120
                }
            }";

            var configuration = new ConfigurationBuilder()
                .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(json)))
                .Build();

            var options = configuration.GetOktaWebApiOptions();

            options.OktaDomain.Should().Be("https://test.okta.com");
            options.AuthorizationServerId.Should().Be("default");
            options.Audience.Should().Be("api://test");
            options.ClockSkew.Should().Be(TimeSpan.FromSeconds(120));
        }

        [Fact]
        public void BindScopesFromJsonConfiguration()
        {
            var json = @"{
                ""Okta"": {
                    ""OktaDomain"": ""https://test.okta.com"",
                    ""ClientId"": ""test-client"",
                    ""ClientSecret"": ""test-secret"",
                    ""Scope"": [""openid"", ""profile"", ""email"", ""offline_access""]
                }
            }";

            var configuration = new ConfigurationBuilder()
                .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(json)))
                .Build();

            var options = configuration.GetOktaMvcOptions();

            options.OktaDomain.Should().Be("https://test.okta.com");
            options.ClientId.Should().Be("test-client");
            options.ClientSecret.Should().Be("test-secret");
            options.Scope.Should().BeEquivalentTo(new[] { "openid", "profile", "email", "offline_access" });
        }

        [Fact]
        public void HaveCorrectDefaultConfigurationSectionName()
        {
            OktaConfigurationExtensions.DefaultConfigurationSection.Should().Be("Okta");
        }

        [Fact]
        public void BindIssuerAndParseOktaDomainForWebApi()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:Issuer", "https://dev-123456.okta.com/oauth2/default" },
            });

            var options = configuration.GetOktaWebApiOptions();

            options.Issuer.Should().Be("https://dev-123456.okta.com/oauth2/default");
            options.OktaDomain.Should().Be("https://dev-123456.okta.com");
            options.AuthorizationServerId.Should().Be("default");
        }

        [Fact]
        public void BindIssuerAndParseOktaDomainForMvc()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:Issuer", "https://dev-123456.okta.com/oauth2/default" },
            });

            var options = configuration.GetOktaMvcOptions();

            options.Issuer.Should().Be("https://dev-123456.okta.com/oauth2/default");
            options.OktaDomain.Should().Be("https://dev-123456.okta.com");
            options.AuthorizationServerId.Should().Be("default");
        }

        [Fact]
        public void BindIssuerWithCustomAuthServerForWebApi()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:Issuer", "https://dev-123456.okta.com/oauth2/aus1234567890" },
            });

            var options = configuration.GetOktaWebApiOptions();

            options.OktaDomain.Should().Be("https://dev-123456.okta.com");
            options.AuthorizationServerId.Should().Be("aus1234567890");
        }

        [Fact]
        public void BindIssuerWithOrgAuthServerForWebApi()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:Issuer", "https://dev-123456.okta.com" },
            });

            var options = configuration.GetOktaWebApiOptions();

            options.OktaDomain.Should().Be("https://dev-123456.okta.com");
            options.AuthorizationServerId.Should().BeEmpty();
        }

        [Fact]
        public void OktaDomainOverridesIssuerParsedValue()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:Issuer", "https://dev-123456.okta.com/oauth2/default" },
                { "Okta:OktaDomain", "https://override.okta.com" },
            });

            var options = configuration.GetOktaWebApiOptions();

            options.OktaDomain.Should().Be("https://override.okta.com");
            options.AuthorizationServerId.Should().Be("default");
        }

        [Fact]
        public void AuthorizationServerIdOverridesIssuerParsedValue()
        {
            var configuration = BuildConfiguration(new Dictionary<string, string>
            {
                { "Okta:Issuer", "https://dev-123456.okta.com/oauth2/default" },
                { "Okta:AuthorizationServerId", "custom-server" },
            });

            var options = configuration.GetOktaWebApiOptions();

            options.OktaDomain.Should().Be("https://dev-123456.okta.com");
            options.AuthorizationServerId.Should().Be("custom-server");
        }

        [Fact]
        public void BindIssuerFromJsonConfiguration()
        {
            var json = @"{
                ""Okta"": {
                    ""Issuer"": ""https://test.okta.com/oauth2/aus123""
                }
            }";

            var configuration = new ConfigurationBuilder()
                .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(json)))
                .Build();

            var options = configuration.GetOktaWebApiOptions();

            options.Issuer.Should().Be("https://test.okta.com/oauth2/aus123");
            options.OktaDomain.Should().Be("https://test.okta.com");
            options.AuthorizationServerId.Should().Be("aus123");
        }

        private static IConfiguration BuildConfiguration(Dictionary<string, string> settings)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
        }
    }
}
