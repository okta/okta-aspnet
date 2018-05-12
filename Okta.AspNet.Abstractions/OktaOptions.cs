using System;

namespace Okta.AspNet.Abstractions
{
    public class OktaOptions
    {
        public static readonly string DefaultAuthorizationServerId = "default";

        public static readonly TimeSpan DefaultClockSkew = TimeSpan.FromMinutes(2);

        public string OrgUrl { get; set; }

        public string ClientId { get; set; }

        public string AuthorizationServerId { get; set; } = DefaultAuthorizationServerId;

        public TimeSpan ClockSkew { get; set; } = DefaultClockSkew;
    }
}
