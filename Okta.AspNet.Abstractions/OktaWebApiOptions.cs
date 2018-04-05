using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.AspNet.Abstractions
{
    public class OktaWebApiOptions : OktaOptions
    {
        public string Audience { get; set; } = "api://default";


    }
}
