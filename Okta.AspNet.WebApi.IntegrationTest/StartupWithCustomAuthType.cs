// <copyright file="StartupWithCustomAuthType.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Provider;
using Owin;

[assembly: OwinStartup(typeof(Okta.AspNet.WebApi.IntegrationTest.StartupWithCustomAuthType))]

namespace Okta.AspNet.WebApi.IntegrationTest
{
    public class StartupWithCustomAuthType
    {
        public HttpMessageHandler HttpMessageHandler { get; set; }

        public void Configuration(IAppBuilder app)
        {
            var oktaDomain = Environment.GetEnvironmentVariable("okta:OktaDomain");
            var jwtProvider = new OAuthBearerAuthenticationProvider
            {
                OnApplyChallenge = context =>
                {
                    context.OwinContext.Response.Headers.Add("myCustomHeader", new[] { "myCustomValue" });
                    return Task.CompletedTask;
                },
            };
            app.UseOktaWebApi("myCustomAuthType", new OktaWebApiOptions()
                {
                    OktaDomain = oktaDomain,
                    OAuthBearerAuthenticationProvider = jwtProvider,
                    BackchannelHttpClientHandler = HttpMessageHandler,
                });
        }
    }
}
