using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Okta.AspNet;
using Microsoft.Owin;
using Newtonsoft.Json;
using System.IO;

namespace OktaAspMvcNetTest
{
    public partial class Startup
    {
        internal static void LogSettings(OpenIdConnectAuthenticationOptions openIdOptions)
        {
            var props = openIdOptions.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).OrderBy(o => o.Name);
            var dics = new Dictionary<string, string>();
            foreach (var prop in props)
            {
                if (dics.ContainsKey(prop.Name))
                    continue;
                try
                {
                    dics.Add(prop.Name, prop.GetValue(openIdOptions)?.ToString() ?? "");
                }
                catch
                {
                    dics.Add(prop.Name, "");
                }
            }
            JsonSerializer serializer = new JsonSerializer();
#if Okta
            using (StreamWriter sw = new StreamWriter(@"C:\\Users\\jabil.100004892\\Desktop\\okta-aspnet\\okta-aspnet\\OktaAspMvcNetTest\\Okta.json"))
#else
            using (StreamWriter sw = new StreamWriter(@"C:\\Users\\jabil.100004892\\Desktop\\okta-aspnet\\okta-aspnet\\OktaAspMvcNetTest\\AzureAD.json"))

#endif
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, dics);
            }

        }

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}