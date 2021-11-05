// <copyright file="OktaWebApiOptions.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Owin.Security.OAuth;
using Owin;

namespace Okta.AspNet
{
    public sealed class OktaWebApiOptions : Abstractions.OktaWebApiOptions
    {
        /// <summary>
        /// Gets ort sets the authentication provider which specifies callback methods invoked by the underlying authentication middleware to enable developer control over the authentication process.
        /// </summary>
        /// <seealso cref="https://docs.microsoft.com/en-us/previous-versions/aspnet/dn253813(v=vs.113)"/>
        public IOAuthBearerAuthenticationProvider OAuthBearerAuthenticationProvider { get; set; }
    }
}
