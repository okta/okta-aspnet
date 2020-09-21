// <copyright file="OktaMvcOptions.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;

namespace Okta.AspNet
{
    public class OktaMvcOptions : Abstractions.OktaWebOptions
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
        /// <value>The redirect uri.</value>
        public string RedirectUri { get; set; }

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
        /// Gets or sets the <see cref="LoginMode"/> to control the login redirect behavior of the middleware.
        /// </summary>
        /// <value>The login mode.</value>
        public LoginMode LoginMode { get; set; } = LoginMode.OktaHosted;

        /// <summary>
        /// Gets or sets a value indicating whether to retrieve additional claims from the UserInfo endpoint after login.
        /// </summary>
        /// <value>The GetClaimsFromUserInfoEndpoint flag.</value>
        public bool GetClaimsFromUserInfoEndpoint { get; set; } = true;

        /// <summary>
        /// Gets or sets the event invoked after the security token has passed validation and a ClaimsIdentity has been generated.
        /// </summary>
        /// <value>The SecurityTokenValidated event.</value>
        public Func<SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions>, Task> SecurityTokenValidated { get; set; } = notification => Task.FromResult(0);

        /// <summary>
        /// Gets or sets the event invoked if exceptions are thrown during request processing.
        /// </summary>
        /// <value>The AuthenticationFailed event.</value>
        public Func<AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions>, Task> AuthenticationFailed { get; set; } = notification => Task.FromResult(0);
    }
}
