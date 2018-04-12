using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.AspNet.Abstractions
{
    public class OktaWebApiOptions : OktaOptions
    {
        public static readonly string DefaultAudience = "api://default";

        public string Audience { get; set; } = DefaultAudience;
    }
}
