﻿// <copyright file="OktaWebOptionsValidator.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text.RegularExpressions;

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
                    "Your Okta URL is missing. Okta URLs should look like: https://{yourOktaDomain}. You can copy your domain from the Okta Developer Console.");
            }

            if (!options.OktaDomain.StartsWith("https://"))
            {
                throw new ArgumentException(
                    "Your Okta URL must start with https. You can copy your domain from the Okta Developer Console.",
                    nameof(options.OktaDomain));
            }

            if (options.OktaDomain.IndexOf("{yourOktaDomain}", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                throw new ArgumentException(
                    "Replace {yourOktaDomain} with your Okta domain. You can copy your domain from the Okta Developer Console.", nameof(options.OktaDomain));
            }

            if (options.OktaDomain.IndexOf("-admin.oktapreview.com", StringComparison.OrdinalIgnoreCase) >= 0 ||
                options.OktaDomain.IndexOf("-admin.okta.com", StringComparison.OrdinalIgnoreCase) >= 0 ||
                options.OktaDomain.IndexOf("-admin.okta-emea.com", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                throw new ArgumentException(
                    "Your Okta domain should not contain -admin. Your domain is: {valueWithoutAdmin}. You can copy your domain from the Okta Developer Console.", nameof(options.OktaDomain));
            }

            if (options.OktaDomain.IndexOf(".com.com", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                throw new ArgumentException(
                    "It looks like there's a typo in your Okta domain. You can copy your domain from the Okta Developer Console.", nameof(options.OktaDomain));
            }

            if (Regex.Matches(options.OktaDomain, "://").Count != 1)
            {
                throw new ArgumentNullException(nameof(options.OktaDomain), "It looks like there's a typo in your Okta domain. You can copy your domain from the Okta Developer Console.");
            }

            if (string.IsNullOrEmpty(options.ClientId))
            {
                throw new ArgumentNullException(
                    nameof(options.ClientId),
                    "Your client ID is missing. You can copy it from the Okta Developer Console in the details for the Application you created.");
            }

            if (options.ClientId.IndexOf("{ClientId}", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                throw new ArgumentNullException(
                    nameof(options.ClientId),
                    "Replace {clientId} with the client ID of your Application. You can copy it from the Okta Developer Console in the details for the Application you created.");
            }

            ValidateInternal((T)options);
        }
    }
}
