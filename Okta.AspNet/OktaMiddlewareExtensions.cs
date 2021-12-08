// <copyright file="OktaMiddlewareExtensions.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
            AddOpenIdConnectAuthentication(app, options);

            return app;
        }

        public static IAppBuilder UseOktaWebApi(this IAppBuilder app, OktaWebApiOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            new OktaWebApiOptionsValidator().Validate(options);
            app.UseJwtBearerAuthentication(JwtOptionsBuilder.BuildJwtBearerAuthenticationOptions(options));

            return app;
        }

        private static void AddOpenIdConnectAuthentication(IAppBuilder app, OktaMvcOptions options)
        {
            // Stop the default behavior of remapping JWT claim names to legacy MS/SOAP claim names
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptionsBuilder(options).BuildOpenIdConnectAuthenticationOptions());
        }
    }
}
