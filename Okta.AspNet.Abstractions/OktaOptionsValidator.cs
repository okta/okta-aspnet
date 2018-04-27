using System;

namespace Okta.AspNet.Abstractions
{
    public class OktaOptionsValidator
    {
        public virtual void ValidateBaseOktaOptions(OktaOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (string.IsNullOrEmpty(options.OrgUrl))
            {
                throw new ArgumentNullException(nameof(options.OrgUrl),
                    "Your Okta Org URL is missing. You can find it in the Okta Developer Console. It'll look like: https://{yourOktaDomain}.com");
            }

            if (!options.OrgUrl.StartsWith("https://"))
            {
                throw new ArgumentException(
                    "Your Okta Org URL must start with https. You can copy your Org URL from the Okta developer dashboard."
                    , nameof(options.OrgUrl));
            }
            
            if (options.OrgUrl.ToUpper().Contains("{yourOktaDomain}".ToUpper()))
            {
                throw new ArgumentException(
                    "You need to copy your Okta Org URL from the Okta developer dashboard."
                    , nameof(options.OrgUrl));
            }

            if (options.OrgUrl.Contains("-admin.oktapreview.com"))
            {
                throw new ArgumentException(
                    "Your Okta Org URL should not contain -admin. You can copy your Org URL from the Okta developer dashboard."
                    , nameof(options.OrgUrl));
            }
            if (options.OrgUrl.Contains(".com.com"))
            {
                throw new ArgumentException(
                    "It looks like there's a typo in your Org URL. You can copy your Org URL from the Okta developer dashboard."
                    , nameof(options.OrgUrl));
            }

            if (string.IsNullOrEmpty(options.ClientId))
            {
                throw new ArgumentNullException(nameof(options.ClientId),
                    "Your Okta Application client ID is missing. You can find it in the Okta Developer Console in the details for the Application you created.");
            }

            if (options.ClientId.ToUpper().Contains("{ClientId}".ToUpper()))
            {
                throw new ArgumentNullException(nameof(options.ClientId),
                  "You need to copy your Client ID from the Okta Developer Console in the details for the Application you created.");
            }
        }
    }
}