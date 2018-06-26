// <copyright file="StrictTokenHandlerShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using FluentAssertions;
using Xunit;

namespace Okta.AspNet.Abstractions.Test
{
    public class StrictTokenHandlerShould
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("bad")]
        public void FailForBadToken(string badToken)
        {
            var fakeOktaWebOptions = new OktaWebOptions
            {
                ClientId = "fake",
                OktaDomain = "example.okta.com",
            };
            var fakeIssuer = "example.okta.com";

            var handler = new StrictTokenHandler();

            Action action = () => handler.ValidateToken(
                badToken,
                new DefaultTokenValidationParameters(fakeOktaWebOptions, fakeIssuer),
                out _);

            action.Should().Throw<ArgumentException>();
        }
    }
}
