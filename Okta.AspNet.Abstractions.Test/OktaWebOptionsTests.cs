using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
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

        [Fact]
        public void Issuer_WhenSetWithDefaultAuthServer_ShouldParseOktaDomainAndAuthorizationServerId()
        {
            // Arrange & Act
            var options = new OktaWebOptions
            {
                Issuer = "https://dev-123456.okta.com/oauth2/default",
            };

            // Assert
            options.OktaDomain.Should().Be("https://dev-123456.okta.com");
            options.AuthorizationServerId.Should().Be("default");
            options.Issuer.Should().Be("https://dev-123456.okta.com/oauth2/default");
        }

        [Fact]
        public void Issuer_WhenSetWithCustomAuthServer_ShouldParseOktaDomainAndAuthorizationServerId()
        {
            // Arrange & Act
            var options = new OktaWebOptions
            {
                Issuer = "https://dev-123456.okta.com/oauth2/aus1234567890",
            };

            // Assert
            options.OktaDomain.Should().Be("https://dev-123456.okta.com");
            options.AuthorizationServerId.Should().Be("aus1234567890");
        }

        [Fact]
        public void Issuer_WhenSetWithOrgAuthServer_ShouldParseOktaDomainWithEmptyAuthorizationServerId()
        {
            // Arrange & Act (org auth server has no /oauth2/ path)
            var options = new OktaWebOptions
            {
                Issuer = "https://dev-123456.okta.com",
            };

            // Assert
            options.OktaDomain.Should().Be("https://dev-123456.okta.com");
            options.AuthorizationServerId.Should().BeEmpty();
        }

        [Fact]
        public void Issuer_WhenSetWithTrailingSlash_ShouldParseCorrectly()
        {
            // Arrange & Act
            var options = new OktaWebOptions
            {
                Issuer = "https://dev-123456.okta.com/oauth2/default/",
            };

            // Assert
            options.OktaDomain.Should().Be("https://dev-123456.okta.com");
            options.AuthorizationServerId.Should().Be("default");
        }

        [Fact]
        public void Issuer_WhenSetWithOktaPreviewDomain_ShouldParseCorrectly()
        {
            // Arrange & Act
            var options = new OktaWebOptions
            {
                Issuer = "https://dev-123456.oktapreview.com/oauth2/default",
            };

            // Assert
            options.OktaDomain.Should().Be("https://dev-123456.oktapreview.com");
            options.AuthorizationServerId.Should().Be("default");
        }

        [Fact]
        public void Issuer_WhenSetWithCustomDomain_ShouldParseCorrectly()
        {
            // Arrange & Act
            var options = new OktaWebOptions
            {
                Issuer = "https://login.mycompany.com/oauth2/default",
            };

            // Assert
            options.OktaDomain.Should().Be("https://login.mycompany.com");
            options.AuthorizationServerId.Should().Be("default");
        }

        [Fact]
        public void OktaDomain_CanOverrideIssuerParsedValue()
        {
            // Arrange & Act
            var options = new OktaWebOptions
            {
                Issuer = "https://dev-123456.okta.com/oauth2/default",
                OktaDomain = "https://different-domain.okta.com",
            };

            // Assert - OktaDomain should be overridden
            options.OktaDomain.Should().Be("https://different-domain.okta.com");
            options.AuthorizationServerId.Should().Be("default");
        }

        [Fact]
        public void AuthorizationServerId_CanOverrideIssuerParsedValue()
        {
            // Arrange & Act
            var options = new OktaWebOptions
            {
                Issuer = "https://dev-123456.okta.com/oauth2/default",
                AuthorizationServerId = "custom-auth-server",
            };

            // Assert - AuthorizationServerId should be overridden
            options.OktaDomain.Should().Be("https://dev-123456.okta.com");
            options.AuthorizationServerId.Should().Be("custom-auth-server");
        }

        [Fact]
        public void Issuer_WhenNull_ShouldNotAffectOtherProperties()
        {
            // Arrange & Act
            var options = new OktaWebOptions
            {
                OktaDomain = "https://dev-123456.okta.com",
                AuthorizationServerId = "custom",
                Issuer = null,
            };

            // Assert
            options.OktaDomain.Should().Be("https://dev-123456.okta.com");
            options.AuthorizationServerId.Should().Be("custom");
            options.Issuer.Should().BeNull();
        }

        [Fact]
        public void Issuer_WhenEmpty_ShouldNotAffectOtherProperties()
        {
            // Arrange & Act
            var options = new OktaWebOptions
            {
                OktaDomain = "https://dev-123456.okta.com",
                AuthorizationServerId = "custom",
                Issuer = string.Empty,
            };

            // Assert
            options.OktaDomain.Should().Be("https://dev-123456.okta.com");
            options.AuthorizationServerId.Should().Be("custom");
        }

        [Fact]
        public void Issuer_WhenInvalidUrl_ShouldNotThrowAndNotModifyProperties()
        {
            // Arrange
            var options = new OktaWebOptions
            {
                OktaDomain = "https://existing.okta.com",
                AuthorizationServerId = "existing",
            };

            // Act - Set invalid URL
            options.Issuer = "not-a-valid-url";

            // Assert - Should not throw and should not modify existing values
            options.OktaDomain.Should().Be("https://existing.okta.com");
            options.AuthorizationServerId.Should().Be("existing");
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