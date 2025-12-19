// <copyright file="OktaMiddlewareExtensions.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.OpenIdConnect;
using Okta.AspNet.Abstractions;
using Owin;

namespace Okta.AspNet
{
    public static class OktaMiddlewareExtensions
    {
        public static IAppBuilder UseOktaMvc(this IAppBuilder app, OktaMvcOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            new OktaMvcOptionsValidator().Validate(options);
            AddOpenIdConnectAuthentication(app, OktaDefaults.MvcAuthenticationType, options);

            return app;
        }

        public static IAppBuilder UseOktaMvc(this IAppBuilder app, string authenticationType, OktaMvcOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (string.IsNullOrEmpty(authenticationType))
            {
                throw new ArgumentNullException(nameof(authenticationType));
            }

            new OktaMvcOptionsValidator().Validate(options);
            AddOpenIdConnectAuthentication(app, authenticationType, options);

            return app;
        }

        /// <summary>
        /// Configures Okta for MVC applications using configuration from <see cref="IConfiguration"/>.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="configuration">The configuration to bind Okta options from.</param>
        /// <param name="sectionName">The configuration section name. Defaults to "Okta".</param>
        /// <returns>The application builder.</returns>
        /// <example>
        /// <code>
        /// public void Configuration(IAppBuilder app)
        /// {
        ///     var config = new ConfigurationBuilder()
        ///         .AddJsonFile("appsettings.json")
        ///         .Build();
        ///
        ///     app.UseOktaMvc(config);
        /// }
        /// </code>
        /// </example>
        public static IAppBuilder UseOktaMvc(this IAppBuilder app, IConfiguration configuration, string sectionName = OktaConfigurationExtensions.DefaultConfigurationSection)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var options = configuration.GetOktaMvcOptions(sectionName);
            return UseOktaMvc(app, options);
        }

        /// <summary>
        /// Configures Okta for MVC applications using configuration from <see cref="IConfiguration"/> with a specified authentication type.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="authenticationType">The authentication type.</param>
        /// <param name="configuration">The configuration to bind Okta options from.</param>
        /// <param name="sectionName">The configuration section name. Defaults to "Okta".</param>
        /// <returns>The application builder.</returns>
        public static IAppBuilder UseOktaMvc(this IAppBuilder app, string authenticationType, IConfiguration configuration, string sectionName = OktaConfigurationExtensions.DefaultConfigurationSection)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (string.IsNullOrEmpty(authenticationType))
            {
                throw new ArgumentNullException(nameof(authenticationType));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var options = configuration.GetOktaMvcOptions(sectionName);
            return UseOktaMvc(app, authenticationType, options);
        }

        public static IAppBuilder UseOktaWebApi(this IAppBuilder app, OktaWebApiOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            new OktaWebApiOptionsValidator().Validate(options);
            app.UseJwtBearerAuthentication(JwtOptionsBuilder.BuildJwtBearerAuthenticationOptions(OktaDefaults.ApiAuthenticationType, options));

            return app;
        }

        public static IAppBuilder UseOktaWebApi(this IAppBuilder app, string authenticationType, OktaWebApiOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (string.IsNullOrEmpty(authenticationType))
            {
                throw new ArgumentNullException(nameof(authenticationType));
            }

            new OktaWebApiOptionsValidator().Validate(options);
            app.UseJwtBearerAuthentication(JwtOptionsBuilder.BuildJwtBearerAuthenticationOptions(authenticationType, options));

            return app;
        }

        /// <summary>
        /// Configures Okta for Web API applications using configuration from <see cref="IConfiguration"/>.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="configuration">The configuration to bind Okta options from.</param>
        /// <param name="sectionName">The configuration section name. Defaults to "Okta".</param>
        /// <returns>The application builder.</returns>
        /// <example>
        /// <code>
        /// public void Configuration(IAppBuilder app)
        /// {
        ///     var config = new ConfigurationBuilder()
        ///         .AddJsonFile("appsettings.json")
        ///         .Build();
        ///
        ///     app.UseOktaWebApi(config);
        /// }
        /// </code>
        /// </example>
        public static IAppBuilder UseOktaWebApi(this IAppBuilder app, IConfiguration configuration, string sectionName = OktaConfigurationExtensions.DefaultConfigurationSection)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var options = configuration.GetOktaWebApiOptions(sectionName);
            return UseOktaWebApi(app, options);
        }

        /// <summary>
        /// Configures Okta for Web API applications using configuration from <see cref="IConfiguration"/> with a specified authentication type.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="authenticationType">The authentication type.</param>
        /// <param name="configuration">The configuration to bind Okta options from.</param>
        /// <param name="sectionName">The configuration section name. Defaults to "Okta".</param>
        /// <returns>The application builder.</returns>
        public static IAppBuilder UseOktaWebApi(this IAppBuilder app, string authenticationType, IConfiguration configuration, string sectionName = OktaConfigurationExtensions.DefaultConfigurationSection)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (string.IsNullOrEmpty(authenticationType))
            {
                throw new ArgumentNullException(nameof(authenticationType));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var options = configuration.GetOktaWebApiOptions(sectionName);
            return UseOktaWebApi(app, authenticationType, options);
        }

        private static void AddOpenIdConnectAuthentication(IAppBuilder app, string authenticationType, OktaMvcOptions options)
        {
            // Stop the default behavior of remapping JWT claim names to legacy MS/SOAP claim names
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptionsBuilder(authenticationType, options).BuildOpenIdConnectAuthenticationOptions());
        }
    }
}
