// <copyright file="OktaMvcOptions.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.AspNet.Abstractions
{
    public class OktaMvcOptions : OktaOptions
    {
        public static readonly string DefaultScope = "openid profile";

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }

        public string PostLogoutRedirectUri { get; set; }

        public string Scope { get; set; } = DefaultScope;

        public bool GetClaimsFromUserInfoEndpoint { get; set; } = false;
    }
}
