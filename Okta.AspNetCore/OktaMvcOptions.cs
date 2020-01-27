// <copyright file="OktaMvcOptions.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Okta.AspNetCore
{
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
        /// Gets or sets the Uri Okta should redirect to process a login.
        /// </summary>
        /// <value>The base Uri. Configure path in CallbackPath.</value>
        public string CallbackUri { get; set; } = OktaDefaults.CallbackUri;

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
        /// Gets or sets the event invoked when an IdToken has been validated and produced an AuthenticationTicket.
        /// </summary>
        /// <value>The OnTokenValidated event.</value>
        public Func<TokenValidatedContext, Task> OnTokenValidated { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Gets or sets the event invoked when user information is retrieved from the UserInfoEndpoint. The <see cref="GetClaimsFromUserInfoEndpoint"/> value must be true when using this event.
        /// </summary>
        /// <value>The OnUserInformationReceived event.</value>
        public Func<UserInformationReceivedContext, Task> OnUserInformationReceived { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Gets or sets the event invoked when an IdToken has been validated and produced an AuthenticationTicket.
        /// </summary>
        /// <value>The OnTokenValidated event.</value>
        public Func<TokenValidatedContext, Task> OnTickedReceived { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Gets or sets the event invoked when an IdToken has been validated and produced an AuthenticationTicket.
        /// </summary>
        /// <value>The OnTokenValidated event.</value>
        public Func<TokenValidatedContext, Task> OnRedirectToIdentityProvider { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Gets or sets the event invoked when an IdToken has been validated and produced an AuthenticationTicket.
        /// </summary>
        /// <value>The OnTokenValidated event.</value>
        public Func<TokenValidatedContext, Task> OnRedirectToIdentityProviderForSignOut { get; set; } = context => Task.CompletedTask;
    }
}