using System;

namespace Okta.AspNet.Abstractions
{
    public class OktaMvcOptionsValidator : OktaOptionsValidator
    {
        protected override void ValidateOptions(OktaOptions options)
        {
            var mvcOptions = (OktaMvcOptions)options;

            if (string.IsNullOrEmpty(mvcOptions.ClientSecret))
            {
                throw new ArgumentNullException(nameof(mvcOptions.ClientSecret),
                    "Your Okta Application client secret is missing. You can find it in the Okta Developer Console in the details for the Application you created.");
            }

            if (mvcOptions.ClientSecret.IndexOf("{ClientSecret}", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                throw new ArgumentException(
                    "You need to copy your client secret from the Okta Developer Console in the details for the Application you created.",
                    nameof(mvcOptions.ClientSecret));
            }

            if (string.IsNullOrEmpty(mvcOptions.RedirectUri))
            {
                throw new ArgumentNullException(
                    nameof(mvcOptions.RedirectUri),
                    "Your Okta Application redirect URI is missing. You can find it in the Okta Developer Console in the details for the Application you created.");
            }
        }
    }
}
