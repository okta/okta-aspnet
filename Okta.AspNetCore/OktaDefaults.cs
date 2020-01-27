﻿// <copyright file="OktaDefaults.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Okta.AspNetCore
{
    public static class OktaDefaults
    {
        public const string MvcAuthenticationScheme = OpenIdConnectDefaults.AuthenticationScheme;

        public const string ApiAuthenticationScheme = JwtBearerDefaults.AuthenticationScheme;

        public const string CallbackPath = "/authorization-code/callback";

        public const string SignOutCallbackPath = "/signout/callback";

        public static readonly string CallbackUri = string.Empty;

        public static readonly string[] Scope = new string[] { "openid", "profile" };
    }
}