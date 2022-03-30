// <copyright file="OktaWebApiOptions.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;

namespace Okta.AspNetCore
{
    /// <summary>
    /// Configuration options for the underlying OIDC middleware.
    /// </summary>
    public sealed class OktaWebApiOptions : AspNet.Abstractions.OktaWebApiOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OktaWebApiOptions"/> class.
        /// </summary>
        public OktaWebApiOptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OktaWebApiOptions"/> class.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        public OktaWebApiOptions(IConfiguration configuration)
        {
            var domain = configuration["Okta:OktaDomain"];
            if (!string.IsNullOrWhiteSpace(domain))
            {
                this.OktaDomain = domain;
            }

            var authServerId = configuration["Okta:AuthorizationServerId"];
            if (!string.IsNullOrWhiteSpace(authServerId))
            {
                this.AuthorizationServerId = authServerId;
            }

            var audience = configuration["Okta:Audience"];
            if (!string.IsNullOrWhiteSpace(audience))
            {
                this.Audience = audience;
            }

            var timeout = configuration["Okta:BackchannelTimeout"];
            if (!string.IsNullOrWhiteSpace(timeout))
            {
                this.BackchannelTimeout = TimeSpan.Parse(timeout);
            }

            var clockSkew = configuration["Okta:ClockSkew"];
            if (!string.IsNullOrWhiteSpace(clockSkew))
            {
                this.ClockSkew = TimeSpan.Parse(clockSkew);
            }
        }

        /// <summary>
        /// Gets or sets the JwtBearerEvents.
        /// </summary>
        /// <seealso cref="https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.jwtbearer.jwtbearerevents"/>
        public JwtBearerEvents JwtBearerEvents { get; set; }
    }
}
