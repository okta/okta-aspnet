// <copyright file="OktaWebOptionsValidator.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Okta.AspNet.Abstractions
{
    public class OktaWebOptionsValidator<T>
        where T : OktaWebOptions
    {
        protected virtual void ValidateInternal(T options)
        {
            return;
        }

        public void Validate(OktaWebOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (string.IsNullOrEmpty(options.OktaDomain))
            {
                throw new ArgumentNullException(
                    nameof(options.OktaDomain),
                    "Your Okta domain is missing. You can find it in the Okta Developer Console. It'll look like: https://dev-12345.oktapreview.com");
            }

            if (!options.OktaDomain.StartsWith("https://"))
            {
                throw new ArgumentException(
                    "Your Okta domain must start with https. You can copy your Okta domain from the Okta developer dashboard.",
                    nameof(options.OktaDomain));
            }

            if (options.OktaDomain.IndexOf("{yourOktaDomain}", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                throw new ArgumentException(
                    "You need to copy your Okta domain from the Okta developer dashboard.", nameof(options.OktaDomain));
            }

            if (options.OktaDomain.IndexOf("-admin.oktapreview.com", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                throw new ArgumentException(
                    "Your Okta domain should not contain -admin. You can copy your Okta domain from the Okta developer dashboard.", nameof(options.OktaDomain));
            }

            if (options.OktaDomain.IndexOf(".com.com", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                throw new ArgumentException(
                    "It looks like there's a typo in your Okta domain. You can copy your Okta domain from the Okta developer dashboard.", nameof(options.OktaDomain));
            }

            if (string.IsNullOrEmpty(options.ClientId))
            {
                throw new ArgumentNullException(
                    nameof(options.ClientId),
                    "Your Okta Application client ID is missing. You can find it in the Okta Developer Console in the details for the Application you created.");
            }

            if (options.ClientId.IndexOf("{ClientId}", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                throw new ArgumentNullException(
                    nameof(options.ClientId),
                    "You need to copy your Client ID from the Okta Developer Console in the details for the Application you created.");
            }

            ValidateInternal((T)options);
        }
    }
}
