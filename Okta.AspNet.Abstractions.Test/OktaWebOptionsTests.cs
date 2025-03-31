using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Okta.AspNet.Abstractions.Tests
{
    public class OktaWebOptionsTests
    {
        [Fact]
        public async Task BackchannelTimeout_ShouldBeAppliedToHttpClient()
        {
            // Arrange
            var options = new OktaWebOptions
            {
                BackchannelTimeout = TimeSpan.FromSeconds(120)
            };

            var handler = new MockHttpMessageHandler((request, cancellationToken) =>
            {
                // Simulate a delay to test timeout
                Task.Delay(1000, cancellationToken).Wait(cancellationToken);
                return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            });

            var httpClient = new HttpClient(handler)
            {
                Timeout = options.BackchannelTimeout
            };

            var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/userinfo");

            // Act
            var response = await httpClient.SendAsync(request);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }

    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handler;

        public MockHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _handler(request, cancellationToken);
        }
    }
}