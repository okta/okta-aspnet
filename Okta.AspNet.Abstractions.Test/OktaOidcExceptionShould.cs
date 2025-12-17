// <copyright file="OktaOidcExceptionShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Okta.AspNet.Abstractions.Tests
{
    public class OktaOidcExceptionShould
    {
        private const string TestOktaDomain = "https://dev-12345.okta.com";
        private const string TestAuthorizationServerId = "default";

        [Fact]
        public void CreateBasicException()
        {
            // Arrange & Act
            var exception = new OktaOidcException("Test error message");

            // Assert
            Assert.Equal("Test error message", exception.Message);
            Assert.Null(exception.InnerException);
        }

        [Fact]
        public void CreateExceptionWithInnerException()
        {
            // Arrange
            var innerException = new InvalidOperationException("Inner error");

            // Act
            var exception = new OktaOidcException("Test error message", innerException);

            // Assert
            Assert.Equal("Test error message", exception.Message);
            Assert.Same(innerException, exception.InnerException);
        }

        [Fact]
        public void CreateDiscoveryException_WithGenericException()
        {
            // Arrange
            var innerException = new InvalidOperationException("IDX20803: Unable to obtain configuration from: 'https://example.okta.com/.well-known/openid-configuration'.");

            // Act
            var exception = OktaOidcException.CreateDiscoveryException(
                innerException,
                TestOktaDomain,
                TestAuthorizationServerId);

            // Assert
            Assert.NotNull(exception);
            Assert.Same(innerException, exception.InnerException);
            Assert.Contains("Failed to retrieve OIDC discovery document", exception.Message);
            Assert.Contains(TestOktaDomain, exception.Message);
            Assert.Contains(TestAuthorizationServerId, exception.Message);
        }

        [Fact]
        public void CreateDiscoveryException_With401Error_ContainsHelpfulMessage()
        {
            // Arrange
            var httpException = new HttpRequestException("Response status code does not indicate success: 401 (Unauthorized).");
            var innerException = new InvalidOperationException("IDX20803: Unable to obtain configuration", httpException);

            // Act
            var exception = OktaOidcException.CreateDiscoveryException(
                innerException,
                TestOktaDomain,
                TestAuthorizationServerId);

            // Assert
            Assert.Contains("401", exception.Message);
            Assert.Contains("Unauthorized", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void CreateDiscoveryException_With403Error_ContainsHelpfulMessage()
        {
            // Arrange
            var httpException = new HttpRequestException("Response status code does not indicate success: 403 (Forbidden).");
            var innerException = new InvalidOperationException("IDX20803: Unable to obtain configuration", httpException);

            // Act
            var exception = OktaOidcException.CreateDiscoveryException(
                innerException,
                TestOktaDomain,
                TestAuthorizationServerId);

            // Assert
            Assert.Contains("403", exception.Message);
            Assert.Contains("Forbidden", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void CreateDiscoveryException_With404Error_ContainsHelpfulMessage()
        {
            // Arrange
            var httpException = new HttpRequestException("Response status code does not indicate success: 404 (Not Found).");
            var innerException = new InvalidOperationException("IDX20803: Unable to obtain configuration", httpException);

            // Act
            var exception = OktaOidcException.CreateDiscoveryException(
                innerException,
                TestOktaDomain,
                TestAuthorizationServerId);

            // Assert
            Assert.Contains("404", exception.Message);
            Assert.Contains("NotFound", exception.Message); // Enum name
        }

        [Fact]
        public void CreateDiscoveryException_ContainsTroubleshootingSteps()
        {
            // Arrange
            var innerException = new InvalidOperationException("IDX20803: Unable to obtain configuration");

            // Act
            var exception = OktaOidcException.CreateDiscoveryException(
                innerException,
                TestOktaDomain,
                TestAuthorizationServerId);

            // Assert
            Assert.Contains("Troubleshooting steps", exception.Message);
            Assert.Contains("OktaDomain", exception.Message);
            Assert.Contains("AuthorizationServerId", exception.Message);
        }

        [Fact]
        public void CreateDiscoveryException_WithNullInnerException_DoesNotThrow()
        {
            // Act
            var exception = OktaOidcException.CreateDiscoveryException(
                null,
                TestOktaDomain,
                TestAuthorizationServerId);

            // Assert
            Assert.NotNull(exception);
            Assert.Null(exception.InnerException);
            Assert.Contains("Failed to retrieve OIDC discovery document", exception.Message);
        }

        [Fact]
        public void CreateDiscoveryException_WithNullOktaDomain_ThrowsArgumentNullException()
        {
            // Arrange
            var innerException = new InvalidOperationException("IDX20803: Unable to obtain configuration");

            // Act & Assert - UrlHelper.CreateIssuerUrl throws if oktaDomain is null
            Assert.Throws<ArgumentNullException>(() => OktaOidcException.CreateDiscoveryException(
                innerException,
                null,
                TestAuthorizationServerId));
        }

        [Fact]
        public void CreateDiscoveryException_HttpStatusCodeExtraction_From500Error()
        {
            // Arrange
            var httpException = new HttpRequestException("Response status code does not indicate success: 500 (Internal Server Error).");
            var innerException = new InvalidOperationException("IDX20803: Unable to obtain configuration", httpException);

            // Act
            var exception = OktaOidcException.CreateDiscoveryException(
                innerException,
                TestOktaDomain,
                TestAuthorizationServerId);

            // Assert
            Assert.Contains("500", exception.Message);
            Assert.Contains("InternalServerError", exception.Message); // Enum name
        }

        [Fact]
        public void CreateDiscoveryException_InnerExceptionIsPreserved()
        {
            // Arrange
            const string originalMessage = "IDX20803: Unable to obtain configuration from: 'https://example.com/.well-known/openid-configuration'.";
            var innerException = new InvalidOperationException(originalMessage);

            // Act
            var exception = OktaOidcException.CreateDiscoveryException(
                innerException,
                TestOktaDomain,
                TestAuthorizationServerId);

            // Assert - inner exception should be preserved
            Assert.Same(innerException, exception.InnerException);
            Assert.Equal(originalMessage, exception.InnerException.Message);
        }

        [Fact]
        public void CreateDiscoveryException_WithWebException_ExtractsStatusCode()
        {
            // Arrange - Using a nested exception hierarchy similar to real-world scenarios
            // Note: The WebException message needs to contain the status code in a parseable format
            var httpException = new HttpRequestException("Response status code does not indicate success: 401 (Unauthorized).");
            var innerException = new InvalidOperationException("IDX20803: Unable to obtain configuration", httpException);

            // Act
            var exception = OktaOidcException.CreateDiscoveryException(
                innerException,
                TestOktaDomain,
                TestAuthorizationServerId);

            // Assert
            Assert.Contains("401", exception.Message);
            Assert.Equal(HttpStatusCode.Unauthorized, exception.HttpStatusCode);
        }

        [Fact]
        public void ExceptionMessage_StartsWithFailedToRetrieve()
        {
            // Arrange
            var innerException = new InvalidOperationException("IDX20803: Unable to obtain configuration");

            // Act
            var exception = OktaOidcException.CreateDiscoveryException(
                innerException,
                TestOktaDomain,
                TestAuthorizationServerId);

            // Assert
            Assert.StartsWith("Failed to retrieve OIDC discovery document", exception.Message);
        }

        [Fact]
        public void CreateDiscoveryException_IncludesDiscoveryUrl()
        {
            // Arrange
            var innerException = new InvalidOperationException("IDX20803: Unable to obtain configuration");

            // Act
            var exception = OktaOidcException.CreateDiscoveryException(
                innerException,
                TestOktaDomain,
                TestAuthorizationServerId);

            // Assert - URL should be constructed from domain and auth server ID
            Assert.Contains(".well-known/openid-configuration", exception.Message);
        }

        [Fact]
        public void CreateDiscoveryException_WithEmptyAuthServerId_UsesOrgAuthServer()
        {
            // Arrange
            var innerException = new InvalidOperationException("IDX20803: Unable to obtain configuration");

            // Act
            var exception = OktaOidcException.CreateDiscoveryException(
                innerException,
                TestOktaDomain,
                string.Empty);

            // Assert
            Assert.NotNull(exception);
            Assert.Contains("Failed to retrieve OIDC discovery document", exception.Message);
        }

        [Fact]
        public void HttpStatusCodeProperty_IsSetCorrectly()
        {
            // Arrange
            var httpException = new HttpRequestException("Response status code does not indicate success: 404 (Not Found).");
            var innerException = new InvalidOperationException("IDX20803: Unable to obtain configuration", httpException);

            // Act
            var exception = OktaOidcException.CreateDiscoveryException(
                innerException,
                TestOktaDomain,
                TestAuthorizationServerId);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, exception.HttpStatusCode);
        }

        [Fact]
        public void OktaDomainProperty_IsSetCorrectly()
        {
            // Arrange
            var innerException = new InvalidOperationException("IDX20803: Unable to obtain configuration");

            // Act
            var exception = OktaOidcException.CreateDiscoveryException(
                innerException,
                TestOktaDomain,
                TestAuthorizationServerId);

            // Assert
            Assert.Equal(TestOktaDomain, exception.OktaDomain);
        }

        [Fact]
        public void AuthorizationServerIdProperty_IsSetCorrectly()
        {
            // Arrange
            var innerException = new InvalidOperationException("IDX20803: Unable to obtain configuration");

            // Act
            var exception = OktaOidcException.CreateDiscoveryException(
                innerException,
                TestOktaDomain,
                TestAuthorizationServerId);

            // Assert
            Assert.Equal(TestAuthorizationServerId, exception.AuthorizationServerId);
        }

        [Fact]
        public void AttemptedUrlProperty_IsSetCorrectly()
        {
            // Arrange
            var innerException = new InvalidOperationException("IDX20803: Unable to obtain configuration");

            // Act
            var exception = OktaOidcException.CreateDiscoveryException(
                innerException,
                TestOktaDomain,
                TestAuthorizationServerId);

            // Assert
            Assert.NotNull(exception.AttemptedUrl);
            Assert.Contains(".well-known/openid-configuration", exception.AttemptedUrl);
        }
    }
}
