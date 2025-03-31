using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Okta.AspNet.Abstractions.Tests
{
    public class OktaWebOptionsTests
    {
        [Fact]
        public async Task ExecuteWithRetryAsync_ShouldRetryOnTransientFailure()
        {
            // Arrange
            var retryCount = 0;
            var handler = new MockHttpMessageHandler((request, cancellationToken) =>
            {
                retryCount++;
                if (retryCount < 3)
                {
                    // Simulate transient failure
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
                }

                // Simulate success on the third attempt
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            });

            var httpClient = new HttpClient(handler);
            var oktaWebOptions = new OktaWebOptions();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/userinfo");

            // Act
            var response = await oktaWebOptions.ExecuteWithRetryAsync(httpClient, request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(3, retryCount); // Ensure it retried twice before succeeding
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