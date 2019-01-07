// <copyright file="OktaWebApiOptions.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Okta.AspNet.Abstractions
{
    public class OktaWebApiOptions : OktaWebOptions
    {
        public static readonly string DefaultAudience = "api://default";

        public string Audience { get; set; } = DefaultAudience;

        [Obsolete("ClientId is no longer required, and has no effect. This property will be removed in the next major release.")]
        public string ClientId { get; set; }
    }
}
