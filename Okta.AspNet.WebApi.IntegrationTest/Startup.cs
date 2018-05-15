using System;
using Microsoft.Owin;
using Okta.AspNet.Abstractions;
using Owin;

[assembly: OwinStartup(typeof(Okta.AspNet.WebApi.IntegrationTest.Startup))]

namespace Okta.AspNet.WebApi.IntegrationTest
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var orgUrl = Environment.GetEnvironmentVariable("okta:OrgUrl");
            var clientId = Environment.GetEnvironmentVariable("okta:ClientId");
            var authorizationServerId = Environment.GetEnvironmentVariable("okta:AuthorizationServerId");
            app.UseOktaWebApi(new OktaWebApiOptions()
                {
                    OrgUrl = orgUrl,
                    ClientId = clientId,
                    AuthorizationServerId = authorizationServerId
                });
        }
    }
}
