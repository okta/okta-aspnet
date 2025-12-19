// <copyright file="OktaConfigurationExtensions.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Okta.AspNetCore
{
    /// <summary>
    /// Extension methods to bind Okta options from <see cref="IConfiguration"/>.
    /// </summary>
    public static class OktaConfigurationExtensions
    {
        /// <summary>
        /// The default configuration section name for Okta settings.
        /// </summary>
        public const string DefaultConfigurationSection = "Okta";

        /// <summary>
        /// Binds the Okta Web API options from the specified configuration section.
        /// </summary>
        /// <param name="configuration">The configuration to bind from.</param>
        /// <param name="sectionName">The configuration section name. Defaults to "Okta".</param>
        /// <returns>The configured <see cref="OktaWebApiOptions"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when configuration is null.</exception>
        /// <example>
        /// <code>
        /// // Using default section name "Okta"
        /// var options = configuration.GetOktaWebApiOptions();
        ///
        /// // Using custom section name
        /// var options = configuration.GetOktaWebApiOptions("MyOktaSettings");
        /// </code>
        /// </example>
        public static OktaWebApiOptions GetOktaWebApiOptions(this IConfiguration configuration, string sectionName = DefaultConfigurationSection)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var section = configuration.GetSection(sectionName);
            var options = new OktaWebApiOptions();

            BindWebOptions(section, options);
            BindWebApiOptions(section, options);

            return options;
        }

        /// <summary>
        /// Binds the Okta MVC options from the specified configuration section.
        /// </summary>
        /// <param name="configuration">The configuration to bind from.</param>
        /// <param name="sectionName">The configuration section name. Defaults to "Okta".</param>
        /// <returns>The configured <see cref="OktaMvcOptions"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when configuration is null.</exception>
        /// <example>
        /// <code>
        /// // Using default section name "Okta"
        /// var options = configuration.GetOktaMvcOptions();
        ///
        /// // Using custom section name
        /// var options = configuration.GetOktaMvcOptions("MyOktaSettings");
        /// </code>
        /// </example>
        public static OktaMvcOptions GetOktaMvcOptions(this IConfiguration configuration, string sectionName = DefaultConfigurationSection)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var section = configuration.GetSection(sectionName);
            var options = new OktaMvcOptions();

            BindWebOptions(section, options);
            BindMvcOptions(section, options);

            return options;
        }

        private static void BindWebOptions(IConfigurationSection section, AspNet.Abstractions.OktaWebOptions options)
        {
            // First, check for Issuer - if set, it will auto-populate OktaDomain and AuthorizationServerId
            var issuer = section[nameof(AspNet.Abstractions.OktaWebOptions.Issuer)];
            if (!string.IsNullOrEmpty(issuer))
            {
                options.Issuer = issuer;
            }

            // OktaDomain can override the parsed value from Issuer if explicitly set
            var oktaDomain = section[nameof(AspNet.Abstractions.OktaWebOptions.OktaDomain)];
            if (!string.IsNullOrEmpty(oktaDomain))
            {
                options.OktaDomain = oktaDomain;
            }

            // AuthorizationServerId can override the parsed value from Issuer if explicitly set
            var authorizationServerId = section[nameof(AspNet.Abstractions.OktaWebOptions.AuthorizationServerId)];
            if (!string.IsNullOrEmpty(authorizationServerId))
            {
                options.AuthorizationServerId = authorizationServerId;
            }

            var clockSkewValue = section[nameof(AspNet.Abstractions.OktaWebOptions.ClockSkew)];
            if (!string.IsNullOrEmpty(clockSkewValue))
            {
                if (int.TryParse(clockSkewValue, out var clockSkewSeconds))
                {
                    options.ClockSkew = TimeSpan.FromSeconds(clockSkewSeconds);
                }
                else if (TimeSpan.TryParse(clockSkewValue, out var clockSkewTimeSpan))
                {
                    options.ClockSkew = clockSkewTimeSpan;
                }
            }

            var backchannelTimeoutValue = section[nameof(AspNet.Abstractions.OktaWebOptions.BackchannelTimeout)];
            if (!string.IsNullOrEmpty(backchannelTimeoutValue))
            {
                if (int.TryParse(backchannelTimeoutValue, out var timeoutSeconds))
                {
                    options.BackchannelTimeout = TimeSpan.FromSeconds(timeoutSeconds);
                }
                else if (TimeSpan.TryParse(backchannelTimeoutValue, out var timeoutTimeSpan))
                {
                    options.BackchannelTimeout = timeoutTimeSpan;
                }
            }
        }

        private static void BindWebApiOptions(IConfigurationSection section, OktaWebApiOptions options)
        {
            var audience = section[nameof(AspNet.Abstractions.OktaWebApiOptions.Audience)];
            if (!string.IsNullOrEmpty(audience))
            {
                options.Audience = audience;
            }
        }

        private static void BindMvcOptions(IConfigurationSection section, OktaMvcOptions options)
        {
            var clientId = section[nameof(OktaMvcOptions.ClientId)];
            if (!string.IsNullOrEmpty(clientId))
            {
                options.ClientId = clientId;
            }

            var clientSecret = section[nameof(OktaMvcOptions.ClientSecret)];
            if (!string.IsNullOrEmpty(clientSecret))
            {
                options.ClientSecret = clientSecret;
            }

            var callbackPath = section[nameof(OktaMvcOptions.CallbackPath)];
            if (!string.IsNullOrEmpty(callbackPath))
            {
                options.CallbackPath = callbackPath;
            }

            var postLogoutRedirectUri = section[nameof(OktaMvcOptions.PostLogoutRedirectUri)];
            if (!string.IsNullOrEmpty(postLogoutRedirectUri))
            {
                options.PostLogoutRedirectUri = postLogoutRedirectUri;
            }

            var getClaimsFromUserInfoEndpoint = section[nameof(OktaMvcOptions.GetClaimsFromUserInfoEndpoint)];
            if (!string.IsNullOrEmpty(getClaimsFromUserInfoEndpoint) && bool.TryParse(getClaimsFromUserInfoEndpoint, out var getClaimsValue))
            {
                options.GetClaimsFromUserInfoEndpoint = getClaimsValue;
            }

            // Bind Scope array
            var scopeSection = section.GetSection(nameof(OktaMvcOptions.Scope));
            var scopes = scopeSection.Get<string[]>();
            if (scopes != null && scopes.Length > 0)
            {
                options.Scope = scopes.ToList();
            }
        }
    }
}
