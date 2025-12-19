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
        /// <remarks>
        /// Use this property to provide an instance of <see cref="JwtBearerEvents"/>.
        /// For dependency injection support, use <see cref="JwtBearerEventsType"/> instead.
        /// If both are set, <see cref="JwtBearerEventsType"/> takes precedence.
        /// </remarks>
        /// <seealso cref="https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.jwtbearer.jwtbearerevents"/>
        public JwtBearerEvents JwtBearerEvents { get; set; }

        /// <summary>
        /// Gets or sets the type of JwtBearerEvents to use for handling authentication events.
        /// </summary>
        /// <remarks>
        /// When set, the events instance will be resolved from the dependency injection container,
        /// allowing constructor injection of services into your custom events class.
        /// This property takes precedence over <see cref="JwtBearerEvents"/> if both are set.
        /// The type must derive from <see cref="JwtBearerEvents"/>.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Register your custom events class
        /// services.AddScoped&lt;CustomJwtBearerEvents&gt;();
        ///
        /// // Configure Okta to use DI for events
        /// services.AddAuthentication()
        ///     .AddOktaWebApi(new OktaWebApiOptions
        ///     {
        ///         OktaDomain = "https://your-domain.okta.com",
        ///         JwtBearerEventsType = typeof(CustomJwtBearerEvents)
        ///     });
        /// </code>
        /// </example>
        /// <seealso cref="https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.authenticationschemeoptions.eventstype"/>
        public Type JwtBearerEventsType { get; set; }
    }
}
