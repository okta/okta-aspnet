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
                OrgUrl = OktaOptionsValidatorHelper.ValidOrgUrl,
                ClientId = clientId,
            };

            Action action = () => new MockOktaOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == nameof(OktaOptions.ClientId));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailIfOrgUrlIsNullOrEmpty(string orgUrl)
        {
            var options = new OktaOptions()
            {
                OrgUrl = orgUrl,
                ClientId = "ClientId",
            };

            Action action = () => new MockOktaOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == nameof(OktaOptions.OrgUrl));
        }

        [Theory]
        [InlineData("http://myOktaDomain.oktapreview.com")]
        [InlineData("httsp://myOktaDomain.oktapreview.com")]
        [InlineData("invalidOrgUrl")]
        public void FailIfOrgUrlIsNotStartingWithHttps(string orgUrl)
        {
            var options = new OktaMvcOptions()
            {
                OrgUrl = orgUrl,
                ClientId = "ClientId",
            };

            Action action = () => new MockOktaOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == nameof(OktaOptions.OrgUrl));
        }

        [Theory]
        [InlineData("https://{Youroktadomain}.com")]
        [InlineData("https://{yourOktaDomain}.com")]
        [InlineData("https://{YourOktaDomain}.com")]
        public void FailIfOrgUrlIsNotDefined(string orgUrl)
        {
            var options = new OktaMvcOptions()
            {
                OrgUrl = orgUrl,
                ClientId = "ClientId",
            };

            Action action = () => new MockOktaOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == nameof(OktaOptions.OrgUrl));
        }

        [Fact]
        public void FailIfOrgUrlIsIncludingAdmin()
        {
            var options = new OktaMvcOptions()
            {
                OrgUrl = "https://myOktaOrg-admin.oktapreview.com",
                ClientId = "ClientId",
            };

            Action action = () => new MockOktaOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == nameof(OktaOptions.OrgUrl));
        }

        [Fact]
        public void FailIfOrgUrlHasTypo()
        {
            var options = new OktaMvcOptions()
            {
                OrgUrl = "https://myOktaDomain.oktapreview.com.com",
                ClientId = "ClientId",
            };

            Action action = () => new MockOktaOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == nameof(OktaOptions.OrgUrl));
        }
    }
}
