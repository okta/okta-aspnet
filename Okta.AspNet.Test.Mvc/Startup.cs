using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Okta.AspNet.Abstractions;
using Owin;

[assembly: OwinStartup(typeof(Okta.AspNet.Test.Startup))]

namespace Okta.AspNet.Test
{
    public class Startup
    {
        // These values are stored in Web.config. Make sure you update them!
        private readonly string clientId = ConfigurationManager.AppSettings["okta:ClientId"];
        private readonly string redirectUri = ConfigurationManager.AppSettings["okta:RedirectUri"];
        private readonly string orgAuthorityUri = ConfigurationManager.AppSettings["okta:OrgAuthorityUri"];
        private readonly string clientSecret = ConfigurationManager.AppSettings["okta:ClientSecret"];
        private readonly string postLogoutRedirectUri = ConfigurationManager.AppSettings["okta:PostLogoutRedirectUri"];

        public void Configuration(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseOktaMvc
            (
                new OktaMvcOptions()
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    OrgAuthorityUri = orgAuthorityUri,
                    RedirectUri = redirectUri,
                    PostLogoutRedirectUri = postLogoutRedirectUri
                }
            );
        }
    }
}
