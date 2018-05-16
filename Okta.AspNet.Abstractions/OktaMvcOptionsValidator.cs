// <copyright file="OktaMvcOptionsValidator.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Okta.AspNet.Abstractions
{
    public class OktaMvcOptionsValidator : OktaOptionsValidator
    {
        protected override void ValidateOptions(OktaOptions options)
        {
            var mvcOptions = (OktaMvcOptions)options;

            if (string.IsNullOrEmpty(mvcOptions.ClientSecret))
            {
                throw new ArgumentNullException(
                    nameof(mvcOptions.ClientSecret),
                    "Your Okta Application client secret is missing. You can find it in the Okta Developer Console in the details for the Application you created.");
            }

            if (mvcOptions.ClientSecret.IndexOf("{ClientSecret}", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                throw new ArgumentException(
                    "You need to copy your client secret from the Okta Developer Console in the details for the Application you created.",
                    nameof(mvcOptions.ClientSecret));
            }

            if (string.IsNullOrEmpty(mvcOptions.RedirectUri))
            {
                throw new ArgumentNullException(
                    nameof(mvcOptions.RedirectUri),
                    "Your Okta Application redirect URI is missing. You can find it in the Okta Developer Console in the details for the Application you created.");
            }
        }
    }
}
