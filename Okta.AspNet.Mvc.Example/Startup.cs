using System.Configuration;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Okta.AspNet.Abstractions;
using Owin;

[assembly: OwinStartup(typeof(Okta.AspNet.Mvc.Example.Startup))]

namespace Okta.AspNet.Mvc.Example
{
    public class Startup
    {
        // These values are stored in Web.config. Make sure you update them!
        private readonly string clientId = ConfigurationManager.AppSettings["okta:ClientId"];
        private readonly string redirectUri = ConfigurationManager.AppSettings["okta:RedirectUri"];
        private readonly string orgUrl = ConfigurationManager.AppSettings["okta:OrgUrl"];
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
                    OrgUrl = orgUrl,
                    RedirectUri = redirectUri,
                    PostLogoutRedirectUri = postLogoutRedirectUri
                }
            );
        }
    }
}
