using System;

namespace Okta.AspNet.Abstractions
{
    public class OktaMvcOptionsValidator : OktaOptionsValidator
    {
        public void Validate(OktaMvcOptions options)
        {
            base.ValidateBaseOktaOptions(options);

            if (string.IsNullOrEmpty(options.ClientSecret))
            {
                throw new ArgumentNullException(nameof(options.ClientSecret),
                    "Your Okta Application client secret is missing. You can find it in the Okta Developer Console in the details for the Application you created.");
            }

            if (options.ClientSecret.ToUpper().Contains("{ClientSecret}".ToUpper()))
            {
                throw new ArgumentNullException(nameof(options.ClientSecret),
                    "You need to copy your client secret from the Okta Developer Console in the details for the Application you created.");
            }

            if (string.IsNullOrEmpty(options.RedirectUri))
            {
                throw new ArgumentNullException(nameof(options.RedirectUri),
                    "Your Okta Application redirect URI is missing. You can find it in the Okta Developer Console in the details for the Application you created.");
            }
        }
    }
}
