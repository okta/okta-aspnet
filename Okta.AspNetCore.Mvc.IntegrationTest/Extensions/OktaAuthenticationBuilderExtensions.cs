
#if Okta
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Okta.AspNetCore;
using Remotion.Linq.Utilities;

namespace Microsoft.AspNetCore.Authentication
{
    public static class OktaAuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddOkta(this AuthenticationBuilder builder)
            => builder.AddOkta(_ => { });

        public static AuthenticationBuilder AddOkta(this AuthenticationBuilder builder, Action<OktaOptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            builder.Services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, ConfigureOktaOptions>();
            builder.AddOpenIdConnect();
            return builder;
        }

        private class ConfigureOktaOptions : IConfigureNamedOptions<OpenIdConnectOptions>
        {
            private readonly OktaOptions _oktaOptions;

            public ConfigureOktaOptions(IOptions<OktaOptions> oktaOptions)
            {
                _oktaOptions = oktaOptions.Value;
            }

            public void Configure(string name, OpenIdConnectOptions options)
            {
                OpenIdConnectOptionsHelper.ConfigureOpenIdConnectOptions(new OktaMvcOptions()
                {
                    ClientSecret = _oktaOptions.ClientSecret,
                    ClientId = _oktaOptions.ClientId,
                    OktaDomain = _oktaOptions.OktaDomain,
                    CallbackPath = _oktaOptions.CallbackPath,
                    PostLogoutRedirectUri = _oktaOptions.PostLogoutUrl
                }, new OpenIdConnectEvents(), options);
            }

            public void Configure(OpenIdConnectOptions options)
            {
                Configure(Options.DefaultName, options);
            }
        }
    }
}
#endif