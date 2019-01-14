// <copyright file="Startup.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Okta.AspNet.WebApi.IntegrationTest.Startup))]

namespace Okta.AspNet.WebApi.IntegrationTest
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var oktaDomain = Environment.GetEnvironmentVariable("okta:OktaDomain");
            var clientId = Environment.GetEnvironmentVariable("okta:ClientId");
            var authorizationServerId = Environment.GetEnvironmentVariable("okta:AuthorizationServerId");
            app.UseOktaWebApi(new OktaWebApiOptions()
                {
                    OktaDomain = oktaDomain,
                    AuthorizationServerId = authorizationServerId,
                });
        }
    }
}
