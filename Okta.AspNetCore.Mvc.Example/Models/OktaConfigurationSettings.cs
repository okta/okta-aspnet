using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Okta.AspNetCore.Mvc.Example.Models
{
    public class OktaConfigurationSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string OrgUrl { get; set; }
    }
}
