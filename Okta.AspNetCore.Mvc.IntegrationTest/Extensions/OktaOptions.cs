#if Okta
namespace Microsoft.AspNetCore.Authentication
{
    public class OktaOptions
    {
        public string ClientId { get; set; }
        
        public string ClientSecret { get; set; }
        
        public string OktaDomain { get; set; }
        
        public string PostLogoutUrl { get; set; }
        
        public string CallbackPath { get; set; }
    }
}
#endif
