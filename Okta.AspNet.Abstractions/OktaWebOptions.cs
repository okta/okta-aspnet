// <copyright file="OktaWebOptions.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Okta.AspNet.Abstractions
{
    public class OktaWebOptions
    {
        public static readonly string DefaultAuthorizationServerId = "default";

        public static readonly TimeSpan DefaultClockSkew = TimeSpan.FromMinutes(2);

        public string OktaDomain { get; set; }

        public string AuthorizationServerId { get; set; } = DefaultAuthorizationServerId;

        public TimeSpan ClockSkew { get; set; } = DefaultClockSkew;
    }
}
