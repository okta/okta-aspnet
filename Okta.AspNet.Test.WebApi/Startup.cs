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
            var authority = ConfigurationManager.AppSettings["okta:OrgUrl"];
            app.UseOktaWebApi(new OktaWebApiOptions() { OrgUrl = authority });
        }
    }
}
