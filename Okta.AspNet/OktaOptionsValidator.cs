using Okta.AspNet.Abstractions;
using System;

namespace Okta.AspNet
{
    public class OktaOptionsValidator
    {
        public void Validate(OktaOptions options)
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

            if (string.IsNullOrEmpty(options.ClientId))
            {
                throw new ArgumentNullException(nameof(options.ClientId),
                    "Your Okta Application client ID is missing. You can find it in the Okta Developer Console in the details for the Application you created.");
            }
        }
    }
}