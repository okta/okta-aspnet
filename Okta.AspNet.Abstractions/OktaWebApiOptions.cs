// <copyright file="OktaWebApiOptions.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Http;

namespace Okta.AspNet.Abstractions
{
    public class OktaWebApiOptions : OktaWebOptions
    {
        public static readonly string DefaultAudience = "api://default";

        public string Audience { get; set; } = DefaultAudience;

        [Obsolete("ClientId is no longer required, and has no effect. This property will be removed in the next major release.")]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the HttpClientHandler used to communicate with Okta.
        /// </summary>
        public HttpClientHandler BackchannelHttpClientHandler { get; set; }

        /// <summary>
        /// Gets or sets timeout value in milliseconds for back channel communications with Okta.
        /// </summary>
        public TimeSpan BackchannelTimeout { get; set; } = TimeSpan.FromSeconds(60);
    }
}
