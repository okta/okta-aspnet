// <copyright file="OktaMvcOptionsPushedAuthorizationBehaviorShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Okta.AspNetCore.Test
{
    public class OktaMvcOptionsPushedAuthorizationBehaviorShould
    {
#if NET9_0_OR_GREATER
        [Fact]
        public void ApplyPushedAuthorizationBehaviorWhenSetToDisable()
        {
            var services = new ServiceCollection();

            var oktaMvcOptions = new OktaMvcOptions
            {
                OktaDomain = "https://test.okta.com",
                ClientId = "test-client-id",
                ClientSecret = "test-client-secret",
                PushedAuthorizationBehavior = PushedAuthorizationBehavior.Disable,
            };

            services.AddAuthentication()
                .AddOktaMvc(oktaMvcOptions);

            var serviceProvider = services.BuildServiceProvider();

            var oidcOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptionsSnapshot<OpenIdConnectOptions>>()
                .Get(OpenIdConnectDefaults.AuthenticationScheme);

            oidcOptions.PushedAuthorizationBehavior.Should().Be(PushedAuthorizationBehavior.Disable);
        }

        [Fact]
        public void ApplyPushedAuthorizationBehaviorWhenSetToRequire()
        {
            var services = new ServiceCollection();

            var oktaMvcOptions = new OktaMvcOptions
            {
                OktaDomain = "https://test.okta.com",
                ClientId = "test-client-id",
                ClientSecret = "test-client-secret",
                PushedAuthorizationBehavior = PushedAuthorizationBehavior.Require,
            };

            services.AddAuthentication()
                .AddOktaMvc(oktaMvcOptions);

            var serviceProvider = services.BuildServiceProvider();

            var oidcOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptionsSnapshot<OpenIdConnectOptions>>()
                .Get(OpenIdConnectDefaults.AuthenticationScheme);

            oidcOptions.PushedAuthorizationBehavior.Should().Be(PushedAuthorizationBehavior.Require);
        }

        [Fact]
        public void NotOverrideDefaultBehaviorWhenNotSet()
        {
            var services = new ServiceCollection();

            var oktaMvcOptions = new OktaMvcOptions
            {
                OktaDomain = "https://test.okta.com",
                ClientId = "test-client-id",
                ClientSecret = "test-client-secret",
                // PushedAuthorizationBehavior not set
            };

            services.AddAuthentication()
                .AddOktaMvc(oktaMvcOptions);

            var serviceProvider = services.BuildServiceProvider();

            var oidcOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptionsSnapshot<OpenIdConnectOptions>>()
                .Get(OpenIdConnectDefaults.AuthenticationScheme);

            // Should use the default value from OpenIdConnectOptions (UseIfAvailable)
            oidcOptions.PushedAuthorizationBehavior.Should().Be(PushedAuthorizationBehavior.UseIfAvailable);
        }
#endif
    }
}
