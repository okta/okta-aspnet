using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.AspNet.Abstractions
{
    public class OktaOptions
    {
        private string authorizationServerId;

        public string OrgUrl { get; set; }

        public string ClientId { get; set; }

        public string AuthorizationServerId
        {
            get
            {
                if (string.IsNullOrEmpty(authorizationServerId))
                {
                    return OrgUrl;
                }
                else
                {
                    return $"{OrgUrl}/oauth2/{AuthorizationServerId}";
                }
            }

            set
            {
                authorizationServerId = value;
            }
        }
    }
}
