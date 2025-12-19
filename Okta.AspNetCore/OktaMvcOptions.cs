// <copyright file="OktaMvcOptions.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Okta.AspNetCore
{
    /// <summary>
    /// The configuration options for the underlying OIDC middleware.
    /// </summary>
    public class OktaMvcOptions : AspNet.Abstractions.OktaWebOptions
    {
        /// <summary>
        /// Gets or sets the client secret of your Okta Application.
        /// </summary>
        /// <value>The client secret.</value>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the client ID of your Okta Application.
        /// </summary>
        /// <value>The client ID.</value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the location Okta should redirect to process a login.
        /// </summary>
        /// <value>The callback path.</value>
        public string CallbackPath { get; set; } = OktaDefaults.CallbackPath;

        /// <summary>
        /// Gets or sets the location Okta should redirect to after logout. If blank, Okta will redirect to the Okta login page.
        /// </summary>
        /// <value>The post-logout redirect URI.</value>
        public string PostLogoutRedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the OAuth 2.0/OpenID Connect scopes to request when logging in.
        /// </summary>
        /// <value>The scope.</value>
        public IList<string> Scope { get; set; } = OktaDefaults.Scope;

        /// <summary>
        /// Gets or sets a value indicating whether to retrieve additional claims from the UserInfo endpoint after login.
        /// </summary>
        /// <value>The GetClaimsFromUserInfoEndpoint flag.</value>
        public bool GetClaimsFromUserInfoEndpoint { get; set; } = true;

        /// <summary>
        /// Gets or sets the OIDC events.
        /// </summary>
        /// <remarks>
        /// Use this property to provide an instance of <see cref="OpenIdConnectEvents"/>.
        /// For dependency injection support, use <see cref="OpenIdConnectEventsType"/> instead.
        /// If both are set, <see cref="OpenIdConnectEventsType"/> takes precedence.
        /// </remarks>
        /// <seealso cref="https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.openidconnect.openidconnectevents"/>
        public OpenIdConnectEvents OpenIdConnectEvents { get; set; }

        /// <summary>
        /// Gets or sets the type of OpenIdConnectEvents to use for handling authentication events.
        /// </summary>
        /// <remarks>
        /// When set, the events instance will be resolved from the dependency injection container,
        /// allowing constructor injection of services into your custom events class.
        /// This property takes precedence over <see cref="OpenIdConnectEvents"/> if both are set.
        /// The type must derive from <see cref="OpenIdConnectEvents"/>.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Register your custom events class
        /// services.AddScoped&lt;CustomOpenIdConnectEvents&gt;();
        ///
        /// // Configure Okta to use DI for events
        /// services.AddAuthentication()
        ///     .AddOktaMvc(new OktaMvcOptions
        ///     {
        ///         OktaDomain = "https://your-domain.okta.com",
        ///         ClientId = "your-client-id",
        ///         ClientSecret = "your-client-secret",
        ///         OpenIdConnectEventsType = typeof(CustomOpenIdConnectEvents)
        ///     });
        /// </code>
        /// </example>
        /// <seealso cref="https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.authenticationschemeoptions.eventstype"/>
        public Type OpenIdConnectEventsType { get; set; }

#if NET9_0_OR_GREATER
        /// <summary>
        /// Gets or sets the behavior for Pushed Authorization Requests (PAR).
        /// </summary>
        /// <value>The pushed authorization behavior. If not set, the default behavior (UseIfAvailable) will be used.</value>
        public PushedAuthorizationBehavior? PushedAuthorizationBehavior { get; set; }
#endif
    }
}
