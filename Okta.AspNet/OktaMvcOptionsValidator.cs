﻿// <copyright file="OktaMvcOptionsValidator.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using Okta.AspNet.Abstractions;

namespace Okta.AspNet
{
    public sealed class OktaMvcOptionsValidator : OktaWebOptionsValidator<OktaMvcOptions>
    {
        protected override void ValidateInternal(OktaMvcOptions options)
        {
            if (string.IsNullOrEmpty(options.ClientSecret))
            {
                throw new ArgumentNullException(
                    nameof(options.ClientSecret),
                    "Your client secret is missing. You can copy it from the Okta Developer Console in the details for the Application you created.");
            }

            if (options.ClientSecret.IndexOf("{ClientSecret}", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                throw new ArgumentException(
                    "Replace {clientSecret} with the client secret of your Application. You can copy it from the Okta Developer Console in the details for the Application you created.",
                    nameof(options.ClientSecret));
            }

            if (string.IsNullOrEmpty(options.RedirectUri))
            {
                throw new ArgumentNullException(
                    nameof(options.RedirectUri),
                    "Your Okta Application redirect URI is missing. You can find it in the Okta Developer Console in the details for the Application you created.");
            }
        }
    }
}
