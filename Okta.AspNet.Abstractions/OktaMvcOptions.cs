using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.AspNet.Abstractions
{
    public class OktaMvcOptions : OktaOptions
    {
        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }

        public string PostLogoutRedirectUri { get; set; }

        public string Scope { get; set; }
    }
}
