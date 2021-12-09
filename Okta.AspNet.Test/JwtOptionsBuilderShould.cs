// <copyright file="JwtOptionsBuilderShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using FluentAssertions;
using Microsoft.Owin.Security.OAuth;
using NSubstitute;
using Xunit;

namespace Okta.AspNet.Test
{
    public class JwtOptionsBuilderShould
    {
        [Fact]
        public void BuildJwtBearerOptions()
        {
            var mockAuthnProvider = Substitute.For<OAuthBearerAuthenticationProvider>();

            var oktaWebApiOptions = new OktaWebApiOptions
            {
                OktaDomain = "http://myoktadomain.com",
                BackchannelTimeout = TimeSpan.FromMinutes(5),
                BackchannelHttpClientHandler = new MockHttpClientHandler(),
                OAuthBearerAuthenticationProvider = mockAuthnProvider,
            };

            var jwtOptions = JwtOptionsBuilder.BuildJwtBearerAuthenticationOptions(oktaWebApiOptions);
            jwtOptions.Should().NotBeNull();
            jwtOptions.Provider.Should().Be(mockAuthnProvider);
        }
    }
}
