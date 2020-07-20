// <copyright file="OktaWebApiOptionsValidatorShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using FluentAssertions;
using Okta.AspNet.Abstractions;
using Xunit;

namespace Okta.AspNet.Test
{
    public class OktaWebApiOptionsValidatorShould
    {
        public const string ValidOktaDomain = "https://myOktaDomain.oktapreview.com";

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailWhenAuhtorizationServerIdNullOrEmpty(string autorizationServer)
        {
            var options = new OktaWebApiOptions()
            {
                OktaDomain = ValidOktaDomain,
                AuthorizationServerId = autorizationServer,
            };

            Action action = () => new OktaWebApiOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == nameof(OktaWebApiOptions.AuthorizationServerId));
        }

        [Theory]
        [InlineData("default")]
        [InlineData("customName")]
        public void NotFailWhenAuhtorizationServerIsNotNullOrEmpty(string autorizationServer)
        {
            var options = new OktaWebApiOptions()
            {
                OktaDomain = ValidOktaDomain,
            };

            // The default AS is `default`, no exceptions expected.
            Action action = () => new OktaWebApiOptionsValidator().Validate(options);
            action.Should().NotThrow();

            options = new OktaWebApiOptions()
            {
                OktaDomain = ValidOktaDomain,
                AuthorizationServerId = autorizationServer,
            };

            action = () => new OktaWebApiOptionsValidator().Validate(options);
            action.Should().NotThrow();
        }
    }
}
