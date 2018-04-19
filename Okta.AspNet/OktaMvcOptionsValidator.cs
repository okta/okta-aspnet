using Okta.AspNet.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Okta.AspNet
{
    public class OktaMvcOptionsValidator : OktaOptionsValidator
    {
        public void Validate(OktaMvcOptions options)
        {
            base.Validate(options);

            if (string.IsNullOrEmpty(options.ClientSecret))
            {
                throw new ArgumentNullException(nameof(options.ClientSecret),
                    "Your Okta Application client secret is missing. You can find it in the Okta Developer Console in the details for the Application you created.");
            }

            if (string.IsNullOrEmpty(options.RedirectUri))
            {
                throw new ArgumentNullException(nameof(options.RedirectUri),
                    "Your Okta Application redirect URI is missing. You can find it in the Okta Developer Console in the details for the Application you created.");
            }
        }
    }
}
