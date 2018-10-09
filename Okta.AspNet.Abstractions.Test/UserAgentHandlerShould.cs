// <copyright file="UserAgentHandlerShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Okta.AspNet.Abstractions.Test.Internal;
using Xunit;

namespace Okta.AspNet.Abstractions.Test
{
    public class UserAgentHandlerShould
    {
        [Theory]
        [InlineData("okta-aspnet")]
        [InlineData("okta-aspnetcore")]
        public async Task BuildUserAgent(string frameworkName)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://foo.com");
            var version = typeof(UserAgentHandlerShould).Assembly.GetName().Version;
            var handler = new UserAgentHandler(frameworkName, version)
            {
                InnerHandler = new TestHandler(),
            };

            var invoker = new HttpMessageInvoker(handler);
            await invoker.SendAsync(httpRequestMessage, CancellationToken.None);

            httpRequestMessage.Headers.GetValues("user-agent").Should().Contain($"{frameworkName}/{version.Major}.{version.Minor}.{version.Build}");
        }
    }
}
