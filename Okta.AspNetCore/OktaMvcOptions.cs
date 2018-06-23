// <copyright file="OktaMvcOptions.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.AspNetCore
{
    public class OktaMvcOptions : AspNet.Abstractions.OktaWebOptions
    {
        public static readonly string DefaultScope = "openid profile";

        public static readonly string DefaultCallbackPath = "/authorization-code/callback";

        public string ClientSecret { get; set; }

        public string CallbackPath { get; set; } = DefaultCallbackPath;

        public string Scope { get; set; } = DefaultScope;

        public bool GetClaimsFromUserInfoEndpoint { get; set; } = false;
    }
}
