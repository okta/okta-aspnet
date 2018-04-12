namespace Okta.AspNet.Abstractions
{
    public class OktaMvcOptions : OktaOptions
    {
        public static readonly string DefaultScope = "openid profile";

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }

        public string PostLogoutRedirectUri { get; set; }

        public string Scope { get; set; } = DefaultScope;

        public bool GetClaimsFromUserInfoEndpoint { get; set; } = false;
    }
}
