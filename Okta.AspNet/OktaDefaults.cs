// <copyright file="OktaDefaults.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.OpenIdConnect;

namespace Okta.AspNet
{
    /// <summary>
    /// Default values used by Okta authentication.
    /// </summary>
    public static class OktaDefaults
    {
        public const string MvcAuthenticationType = OpenIdConnectAuthenticationDefaults.AuthenticationType;

        public const string ApiAuthenticationType = OAuthDefaults.AuthenticationType;

        public static readonly string[] Scope = new string[] { "openid", "profile" };
    }
}
