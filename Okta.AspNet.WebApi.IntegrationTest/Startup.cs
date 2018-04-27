using System.Configuration;
using Microsoft.Owin;
using Okta.AspNet.Abstractions;
using Owin;

[assembly: OwinStartup(typeof(Okta.AspNet.Test.WebApi.Startup))]

namespace Okta.AspNet.Test.WebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var orgUrl = ConfigurationManager.AppSettings["okta:OrgUrl"];
            var clientId = ConfigurationManager.AppSettings["okta:ClientId"];
            var authorizationServerId = ConfigurationManager.AppSettings["okta:AuthorizationServerId"];
            app.UseOktaWebApi(new OktaWebApiOptions()
                {
                    OrgUrl = orgUrl,
                    ClientId = clientId,
                    AuthorizationServerId = authorizationServerId
                });
        }
    }
}
