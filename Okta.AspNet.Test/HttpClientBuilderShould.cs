// <copyright file="HttpClientBuilderShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Okta.AspNet.Test
{
    public class HttpClientBuilderShould
    {
        [Fact]
        public async Task InvokeCustomHandler()
        {
            var handler = new MockHttpClientHandler();

            var options = new OktaWebApiOptions();
            options.OktaDomain = "https://test.okta.com";
            options.BackchannelHttpClientHandler = handler;
            options.BackchannelTimeout = TimeSpan.FromMinutes(5);

            options.BackchannelHttpClientHandler.Should().NotBeNull();

            var client = HttpClientBuilder.CreateClient(options);

            var response = await client.GetAsync("http://www.okta.com");

            handler.NumberOfCalls.Should().BeGreaterThan(0);
            client.Timeout.Should().Be(TimeSpan.FromMinutes(5));
        }
    }
}
