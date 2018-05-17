// <copyright file="OktaMvcOptionsValidatorShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using FluentAssertions;
using Xunit;

namespace Okta.AspNet.Abstractions.Test
{
    public class OktaMvcOptionsValidatorShould
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailWhenClientSecretIsNullOrEmpty(string clientSecret)
        {
            var options = new OktaMvcOptions()
            {
                OrgUrl = OktaOptionsValidatorHelper.ValidOrgUrl,
                ClientId = "ClientId",
                ClientSecret = clientSecret,
            };

            Action action = () => new OktaMvcOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == nameof(OktaMvcOptions.ClientSecret));
        }

        [Fact]
        public void FailWhenClientSecretIsNotDefined()
        {
            var options = new OktaMvcOptions()
            {
                OrgUrl = OktaOptionsValidatorHelper.ValidOrgUrl,
                ClientId = "ClientId",
                ClientSecret = "{ClientSecret}",
            };

            Action action = () => new OktaMvcOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == nameof(OktaMvcOptions.ClientSecret));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailWhenRedirectUriIsNullOrEmpty(string redirectUri)
        {
            var options = new OktaMvcOptions()
                {
                    OrgUrl = OktaOptionsValidatorHelper.ValidOrgUrl,
                    ClientId = "ClientId",
                    ClientSecret = "ClientSecret",
                    RedirectUri = redirectUri,
                };

            Action action = () => new OktaMvcOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == nameof(OktaMvcOptions.RedirectUri));
        }

        [Fact]
        public void NotThrowWhenParamsAreProvided()
        {
            var options = new OktaMvcOptions()
            {
                OrgUrl = OktaOptionsValidatorHelper.ValidOrgUrl,
                ClientId = "ClientId",
                ClientSecret = "ClientSecret",
                RedirectUri = "RedirectUri",
            };

            Action action = () => new OktaMvcOptionsValidator().Validate(options);
            action.Should().NotThrow();
        }
    }
}
