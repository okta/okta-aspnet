// <copyright file="OktaMvcOptionsValidator.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Okta.AspNetCore
{
    public sealed class OktaMvcOptionsValidator : AspNet.Abstractions.OktaWebOptionsValidator<OktaMvcOptions>
    {
        protected override void ValidateInternal(OktaMvcOptions options)
        {
            if (string.IsNullOrEmpty(options.ClientSecret))
            {
                throw new ArgumentNullException(
                    nameof(options.ClientSecret),
                    "Your Okta Application client secret is missing. You can find it in the Okta Developer Console in the details for the Application you created.");
            }

            if (options.ClientSecret.IndexOf("{ClientSecret}", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                throw new ArgumentException(
                    "You need to copy your client secret from the Okta Developer Console in the details for the Application you created.",
                    nameof(options.ClientSecret));
            }

            if (string.IsNullOrEmpty(options.CallbackPath))
            {
                throw new ArgumentNullException(
                    nameof(options.CallbackPath),
                    "Your Okta Application callback path is missing. It should match the path of the redirect URI you specified in the Okta Developer Console for this application.");
            }
        }
    }
}
