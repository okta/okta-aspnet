// <copyright file="OktaOptionsValidatorShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using FluentAssertions;
using Xunit;

namespace Okta.AspNet.Abstractions.Test
{
    public class OktaOptionsValidatorShould
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailWhenClientIdIsNullOrEmpty(string clientId)
        {
            var options = new OktaOptions()
            {
                OktaDomain = OktaOptionsValidatorHelper.ValidOktaDomain,
                ClientId = clientId,
            };

            Action action = () => new MockOktaOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == nameof(OktaOptions.ClientId));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailIfOktaDomainIsNullOrEmpty(string oktaDomain)
        {
            var options = new OktaOptions()
            {
                OktaDomain = oktaDomain,
                ClientId = "ClientId",
            };

            Action action = () => new MockOktaOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == nameof(OktaOptions.OktaDomain));
        }

        [Theory]
        [InlineData("http://myOktaDomain.oktapreview.com")]
        [InlineData("httsp://myOktaDomain.oktapreview.com")]
        [InlineData("invalidOktaDomain")]
        public void FailIfOktaDomainIsNotStartingWithHttps(string oktaDomain)
        {
            var options = new OktaMvcOptions()
            {
                OktaDomain = oktaDomain,
                ClientId = "ClientId",
            };

            Action action = () => new MockOktaOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == nameof(OktaOptions.OktaDomain));
        }

        [Theory]
        [InlineData("https://{Youroktadomain}")]
        [InlineData("https://{yourOktaDomain}")]
        [InlineData("https://{YourOktaDomain}")]
        public void FailIfOktaDomainIsNotDefined(string oktaDomain)
        {
            var options = new OktaMvcOptions()
            {
                OktaDomain = oktaDomain,
                ClientId = "ClientId",
            };

            Action action = () => new MockOktaOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == nameof(OktaOptions.OktaDomain));
        }

        [Fact]
        public void FailIfOktaDomainIsIncludingAdmin()
        {
            var options = new OktaMvcOptions()
            {
                OktaDomain = "https://myOktaOrg-admin.oktapreview.com",
                ClientId = "ClientId",
            };

            Action action = () => new MockOktaOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == nameof(OktaOptions.OktaDomain));
        }

        [Fact]
        public void FailIfOktaDomainHasTypo()
        {
            var options = new OktaMvcOptions()
            {
                OktaDomain = "https://myOktaDomain.oktapreview.com.com",
                ClientId = "ClientId",
            };

            Action action = () => new MockOktaOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == nameof(OktaOptions.OktaDomain));
        }
    }
}
