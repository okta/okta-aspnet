#if Okta

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Okta.AspNet;
using Microsoft.Owin;
using Newtonsoft.Json;
using System.IO;
using Okta.AspNet.Abstractions;

namespace OktaAspMvcNetTest
{
    public partial class Startup
    {
        private static readonly string clientId = ConfigurationManager.AppSettings["Okta:ClientId"];
        private static readonly string domain = ConfigurationManager.AppSettings["Okta:Domain"];
        private static readonly string clientSecret = ConfigurationManager.AppSettings["Okta:ClientSecret"];
        private static readonly string postLogoutRedirectUri = ConfigurationManager.AppSettings["Okta:PostLogoutRedirectUri"];

        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            var opt = new OktaMvcOptions()
            {
                ClientSecret = clientSecret,
                ClientId = clientId,
                OktaDomain = domain,
                PostLogoutRedirectUri = postLogoutRedirectUri
            };
            var oidOptions = new OpenIdConnectAuthenticationOptions();
            ConfigureOpenIdConnectOptions(opt, oidOptions);
            //LogSettings(oidOptions);

            app.UseOpenIdConnectAuthentication(oidOptions);

        }

        public static void ConfigureOpenIdConnectOptions(OktaMvcOptions oktaMvcOptions,
            OpenIdConnectAuthenticationOptions oidcOptions)
        {
            oidcOptions.ClientId = oktaMvcOptions.ClientId;
            oidcOptions.ClientSecret = oktaMvcOptions.ClientSecret;
            oidcOptions.Authority = domain;
            oidcOptions.CallbackPath = new PathString("");
            oidcOptions.PostLogoutRedirectUri = oktaMvcOptions.PostLogoutRedirectUri;
            oidcOptions.ResponseType = "code id_token";
            oidcOptions.SecurityTokenValidator = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            oidcOptions.UseTokenLifetime = true;
            oidcOptions.RedirectUri = "https://localhost:44365/";
            oidcOptions.Scope = "openid profile";
            oidcOptions.TokenValidationParameters = new DefaultTokenValidationParameters(oktaMvcOptions, "Okta")
            {
                ValidAudience = oktaMvcOptions.ClientId,
                NameClaimType = "name",
            };
        }

    }
}

#endif
