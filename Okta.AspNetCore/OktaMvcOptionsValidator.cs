// <copyright file="OktaMvcOptionsValidator.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Okta.AspNetCore
{
    /// <summary>
    /// Validates Okta configuration.
    /// </summary>
    public sealed class OktaMvcOptionsValidator : AspNet.Abstractions.OktaWebOptionsValidator<OktaMvcOptions>
    {
        /// <summary>
        /// Validates MVC configuration.
        /// </summary>
        /// <param name="options">The Okta MVC options.</param>
        protected override void ValidateInternal(OktaMvcOptions options)
        {
            if (string.IsNullOrEmpty(options.ClientId))
            {
                throw new ArgumentNullException(
                    nameof(options.ClientId),
                    "Your client ID is missing. You can copy it from the Okta Developer Console in the details for the Application you created. Follow these instructions to find it: https://bit.ly/finding-okta-app-credentials");
            }

            if (options.ClientId.IndexOf("{ClientId}", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                throw new ArgumentNullException(
                    nameof(options.ClientId),
                    "Replace {clientId} with the client ID of your Application. You can copy it from the Okta Developer Console in the details for the Application you created. Follow these instructions to find it: https://bit.ly/finding-okta-app-credentials");
            }

            if (string.IsNullOrEmpty(options.ClientSecret))
            {
                throw new ArgumentNullException(
                    nameof(options.ClientSecret),
                    "Your client secret is missing. You can copy it from the Okta Developer Console in the details for the Application you created. Follow these instructions to find it: https://bit.ly/finding-okta-app-credentials");
            }

            if (options.ClientSecret.IndexOf("{ClientSecret}", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                throw new ArgumentException(
                    "Replace {clientSecret} with the client secret of your Application. You can copy it from the Okta Developer Console in the details for the Application you created. Follow these instructions to find it: https://bit.ly/finding-okta-app-credentials",
                    nameof(options.ClientSecret));
            }

            if (string.IsNullOrEmpty(options.CallbackPath))
            {
                throw new ArgumentNullException(
                    nameof(options.CallbackPath),
                    "Your Okta Application callback path is missing. It should match the path of the redirect URI you specified in the Okta Developer Console for this application.");
            }

            if (options.BackchannelHttpHandler == null)
            {
                throw new ArgumentNullException(
                    nameof(options.BackchannelHttpHandler));
            }
        }
    }
}
