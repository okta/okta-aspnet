// <copyright file="OktaWebOptionsValidatorShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using FluentAssertions;
using Xunit;

namespace Okta.AspNet.Abstractions.Test
{
    public class OktaWebOptionsValidatorShould
    {
        public const string ValidOktaDomain = "https://myOktaDomain.oktapreview.com";

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailWhenClientIdIsNullOrEmpty(string clientId)
        {
            var options = new OktaWebOptions()
            {
                OktaDomain = ValidOktaDomain,
                ClientId = clientId,
            };

            Action action = () => new OktaWebOptionsValidator<OktaWebOptions>().Validate(options);
            action.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == nameof(OktaWebOptions.ClientId));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailIfOktaDomainIsNullOrEmpty(string oktaDomain)
        {
            var options = new OktaWebOptions()
            {
                OktaDomain = oktaDomain,
                ClientId = "ClientId",
            };

            Action action = () => new OktaWebOptionsValidator<OktaWebOptions>().Validate(options);
            action.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == nameof(OktaWebOptions.OktaDomain));
        }

        [Theory]
        [InlineData("http://myOktaDomain.oktapreview.com")]
        [InlineData("httsp://myOktaDomain.oktapreview.com")]
        [InlineData("invalidOktaDomain")]
        public void FailIfOktaDomainIsNotStartingWithHttps(string oktaDomain)
        {
            var options = new OktaWebOptions()
            {
                OktaDomain = oktaDomain,
                ClientId = "ClientId",
            };

            Action action = () => new OktaWebOptionsValidator<OktaWebOptions>().Validate(options);
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == nameof(OktaWebOptions.OktaDomain));
        }

        [Theory]
        [InlineData("https://{Youroktadomain}")]
        [InlineData("https://{yourOktaDomain}")]
        [InlineData("https://{YourOktaDomain}")]
        public void FailIfOktaDomainIsNotDefined(string oktaDomain)
        {
            var options = new OktaWebOptions()
            {
                OktaDomain = oktaDomain,
                ClientId = "ClientId",
            };

            Action action = () => new OktaWebOptionsValidator<OktaWebOptions>().Validate(options);
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == nameof(OktaWebOptions.OktaDomain));
        }

        [Fact]
        public void FailIfOktaDomainIsIncludingAdmin()
        {
            var options = new OktaWebOptions()
            {
                OktaDomain = "https://myOktaOrg-admin.oktapreview.com",
                ClientId = "ClientId",
            };

            Action action = () => new OktaWebOptionsValidator<OktaWebOptions>().Validate(options);
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == nameof(OktaWebOptions.OktaDomain));
        }

        [Fact]
        public void FailIfOktaDomainHasTypo()
        {
            var options = new OktaWebOptions()
            {
                OktaDomain = "https://myOktaDomain.oktapreview.com.com",
                ClientId = "ClientId",
            };

            Action action = () => new OktaWebOptionsValidator<OktaWebOptions>().Validate(options);
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == nameof(OktaWebOptions.OktaDomain));
        }
    }
}
