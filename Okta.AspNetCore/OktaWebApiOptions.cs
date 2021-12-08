// <copyright file="OktaWebApiOptions.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Okta.AspNetCore
{
    /// <summary>
    /// Configuration options for the underlying OIDC middleware.
    /// </summary>
    public sealed class OktaWebApiOptions : AspNet.Abstractions.OktaWebApiOptions
    {
        /// <summary>
        /// Gets or sets the JwtBearerEvents.
        /// </summary>
        /// <seealso cref="https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.jwtbearer.jwtbearerevents"/>
        public JwtBearerEvents JwtBearerEvents { get; set; }
    }
}
