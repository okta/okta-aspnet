// <copyright file="UrlHelper.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.AspNet.Abstractions
{
    public static class UrlHelper
    {
        public static string CreateIssuerUrl(string oktaDomain, string authorizationServerId)
        {
            if (string.IsNullOrEmpty(oktaDomain))
            {
                throw new ArgumentNullException(nameof(oktaDomain));
            }

            if (string.IsNullOrEmpty(authorizationServerId))
            {
                return oktaDomain;
            }

            return $"{EnsureTrailingSlash(oktaDomain)}oauth2/{authorizationServerId}";
        }

        /// <summary>
        /// Ensures that this URI ends with a trailing slash <c>/</c>.
        /// </summary>
        /// <param name="uri">The URI string.</param>
        /// <returns>The URI string, appended with <c>/</c> if necessary.</returns>
        public static string EnsureTrailingSlash(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                throw new ArgumentNullException(nameof(uri));
            }

            return uri.EndsWith("/")
                ? uri
                : $"{uri}/";
        }
    }
}
