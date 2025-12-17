// <copyright file="OktaOidcException.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net;
using System.Text;

namespace Okta.AspNet.Abstractions
{
    /// <summary>
    /// Exception thrown when OIDC discovery or configuration operations fail.
    /// Provides detailed error information to help developers troubleshoot configuration issues.
    /// </summary>
    public class OktaOidcException : Exception
    {
        /// <summary>
        /// Gets the HTTP status code returned by the server, if available.
        /// </summary>
        public HttpStatusCode? HttpStatusCode { get; }

        /// <summary>
        /// Gets the URL that was being accessed when the error occurred.
        /// </summary>
        public string AttemptedUrl { get; }

        /// <summary>
        /// Gets the configured Okta domain.
        /// </summary>
        public string OktaDomain { get; }

        /// <summary>
        /// Gets the configured Authorization Server ID.
        /// </summary>
        public string AuthorizationServerId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OktaOidcException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public OktaOidcException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OktaOidcException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public OktaOidcException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OktaOidcException"/> class with detailed context.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="httpStatusCode">The HTTP status code, if available.</param>
        /// <param name="attemptedUrl">The URL that was being accessed.</param>
        /// <param name="oktaDomain">The configured Okta domain.</param>
        /// <param name="authorizationServerId">The configured Authorization Server ID.</param>
        public OktaOidcException(
            string message,
            Exception innerException,
            HttpStatusCode? httpStatusCode,
            string attemptedUrl,
            string oktaDomain,
            string authorizationServerId)
            : base(message, innerException)
        {
            HttpStatusCode = httpStatusCode;
            AttemptedUrl = attemptedUrl;
            OktaDomain = oktaDomain;
            AuthorizationServerId = authorizationServerId;
        }

        /// <summary>
        /// Creates an OktaOidcException for a discovery document fetch failure.
        /// </summary>
        /// <param name="innerException">The original exception that occurred.</param>
        /// <param name="oktaDomain">The configured Okta domain.</param>
        /// <param name="authorizationServerId">The configured Authorization Server ID.</param>
        /// <returns>A new OktaOidcException with detailed error information.</returns>
        public static OktaOidcException CreateDiscoveryException(
            Exception innerException,
            string oktaDomain,
            string authorizationServerId)
        {
            var issuerUrl = UrlHelper.CreateIssuerUrl(oktaDomain, authorizationServerId);
            var discoveryUrl = $"{issuerUrl}/.well-known/openid-configuration";
            var httpStatusCode = ExtractHttpStatusCode(innerException);

            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("Failed to retrieve OIDC discovery document from Okta.");
            messageBuilder.AppendLine();

            if (httpStatusCode.HasValue)
            {
                messageBuilder.AppendLine($"HTTP Status: {(int)httpStatusCode.Value} ({httpStatusCode.Value})");
            }

            messageBuilder.AppendLine($"Discovery URL: {discoveryUrl}");
            messageBuilder.AppendLine();
            messageBuilder.AppendLine("Current Configuration:");
            messageBuilder.AppendLine($"  OktaDomain: {oktaDomain ?? "(not set)"}");
            messageBuilder.AppendLine($"  AuthorizationServerId: {authorizationServerId ?? "(not set, using Org Authorization Server)"}");
            messageBuilder.AppendLine();
            messageBuilder.AppendLine("Common causes:");

            if (httpStatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                messageBuilder.AppendLine("  - The Authorization Server ID may be incorrect");
                messageBuilder.AppendLine("  - The Okta domain may be incorrect");
                messageBuilder.AppendLine("  - API access may not be enabled for your Okta org");
            }
            else if (httpStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                messageBuilder.AppendLine("  - The OktaDomain is incorrect or does not exist");
                messageBuilder.AppendLine("  - The AuthorizationServerId does not exist");
                messageBuilder.AppendLine("  - Check that you're using the correct Okta domain (e.g., 'https://dev-12345.okta.com')");
            }
            else if (httpStatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                messageBuilder.AppendLine("  - Access to the authorization server is forbidden");
                messageBuilder.AppendLine("  - Check your Okta org's API access policies");
            }
            else
            {
                messageBuilder.AppendLine("  1. Invalid OktaDomain - Verify it matches your Okta Developer Console");
                messageBuilder.AppendLine("  2. Incorrect AuthorizationServerId - Use 'default' for the default authorization server");
                messageBuilder.AppendLine("  3. Network connectivity issues - Ensure your application can reach Okta's servers");
                messageBuilder.AppendLine("  4. Firewall or proxy blocking the request");
            }

            messageBuilder.AppendLine();
            messageBuilder.AppendLine("Troubleshooting steps:");
            messageBuilder.AppendLine($"  1. Verify the discovery URL is accessible: {discoveryUrl}");
            messageBuilder.AppendLine("  2. Check your OktaDomain in the Okta Admin Console under Settings > Account");
            messageBuilder.AppendLine("  3. Verify AuthorizationServerId under Security > API > Authorization Servers");
            messageBuilder.AppendLine("  4. For more help: https://bit.ly/finding-okta-domain");

            return new OktaOidcException(
                messageBuilder.ToString(),
                innerException,
                httpStatusCode,
                discoveryUrl,
                oktaDomain,
                authorizationServerId);
        }

        /// <summary>
        /// Extracts HTTP status code from the exception if available.
        /// </summary>
        /// <param name="exception">The exception to extract status code from.</param>
        /// <returns>The HTTP status code, or null if not available.</returns>
        private static HttpStatusCode? ExtractHttpStatusCode(Exception exception)
        {
            if (exception == null)
            {
                return null;
            }

            // Check the exception message for common HTTP status codes
            var message = exception.Message ?? string.Empty;
            var innerMessage = exception.InnerException?.Message ?? string.Empty;
            var combinedMessage = $"{message} {innerMessage}";

            // Look for status code patterns like "401" or "Unauthorized"
            if (combinedMessage.Contains("401") || combinedMessage.Contains("Unauthorized"))
            {
                return System.Net.HttpStatusCode.Unauthorized;
            }

            if (combinedMessage.Contains("403") || combinedMessage.Contains("Forbidden"))
            {
                return System.Net.HttpStatusCode.Forbidden;
            }

            if (combinedMessage.Contains("404") || combinedMessage.Contains("Not Found"))
            {
                return System.Net.HttpStatusCode.NotFound;
            }

            if (combinedMessage.Contains("500") || combinedMessage.Contains("Internal Server Error"))
            {
                return System.Net.HttpStatusCode.InternalServerError;
            }

            if (combinedMessage.Contains("502") || combinedMessage.Contains("Bad Gateway"))
            {
                return System.Net.HttpStatusCode.BadGateway;
            }

            if (combinedMessage.Contains("503") || combinedMessage.Contains("Service Unavailable"))
            {
                return System.Net.HttpStatusCode.ServiceUnavailable;
            }

            // Try to find any 3-digit status code in the message
            var match = System.Text.RegularExpressions.Regex.Match(combinedMessage, @"\b([4-5]\d{2})\b");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int statusCode))
            {
                return (HttpStatusCode)statusCode;
            }

            return null;
        }
    }
}
