// <copyright file="OktaMvcOptionsValidatorShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using FluentAssertions;
using Xunit;

namespace Okta.AspNetCore.Test
{
    public class OktaMvcOptionsValidatorShould
    {
        public const string ValidOktaDomain = "https://myOktaDomain.oktapreview.com";

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailWhenClientIdIsNullOrEmpty(string clientId)
        {
            var options = new OktaMvcOptions()
            {
                OktaDomain = ValidOktaDomain,
                ClientId = clientId,
            };

            Action action = () => new OktaMvcOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == nameof(OktaMvcOptions.ClientId));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailWhenClientSecretIsNullOrEmpty(string clientSecret)
        {
            var options = new OktaMvcOptions()
            {
                OktaDomain = ValidOktaDomain,
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
                OktaDomain = ValidOktaDomain,
                ClientId = "ClientId",
                ClientSecret = "{ClientSecret}",
            };

            Action action = () => new OktaMvcOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == nameof(OktaMvcOptions.ClientSecret));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailWhenCallbackPathIsNullOrEmpty(string badCallbackPath)
        {
            var options = new OktaMvcOptions()
            {
                OktaDomain = ValidOktaDomain,
                ClientId = "ClientId",
                ClientSecret = "ClientSecret",
                CallbackPath = badCallbackPath,
            };

            Action action = () => new OktaMvcOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == nameof(OktaMvcOptions.CallbackPath));
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailWhenNameClaimTypeIsNullOrEmpty(string badNameClaimType)
        {
            var options = new OktaMvcOptions()
            {
                OktaDomain = ValidOktaDomain,
                ClientId = "ClientId",
                ClientSecret = "ClientSecret",
                NameClaimType = badNameClaimType,
            };

            Action action = () => new OktaMvcOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == nameof(OktaMvcOptions.NameClaimType));
        }

        [Fact]
        public void NotThrowWhenParamsAreProvided()
        {
            var options = new OktaMvcOptions()
            {
                OktaDomain = ValidOktaDomain,
                ClientId = "ClientId",
                ClientSecret = "ClientSecret",
                CallbackPath = "/some/path",
            };

            Action action = () => new OktaMvcOptionsValidator().Validate(options);
            action.Should().NotThrow();
        }
    }
}
