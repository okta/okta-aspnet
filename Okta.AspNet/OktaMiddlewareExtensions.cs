using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OpenIdConnect;
using Okta.AspNet.Abstractions;
using Owin;
using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Notifications;
using System.Linq;

namespace Okta.AspNet
{
    public static class OktaMiddlewareExtensions
    {
        public static IAppBuilder UseOktaMvc(this IAppBuilder app, OktaMvcOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            ValidateOktaMvcOptions(options);

            AddOpenIdConnectAuthentication(app, options);

            return app;
        }

        public static IAppBuilder UseOktaWebApi(this IAppBuilder app, OktaWebApiOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            ValidateOktaOptions(options);
            AddJwtBearerAuthentication(app, options);

            return app;
        }

        private static void ValidateOktaOptions(OktaOptions options)
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

        private static void ValidateOktaMvcOptions(OktaMvcOptions options)
        {
            ValidateOktaOptions(options);

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

        private static void AddJwtBearerAuthentication(IAppBuilder app, OktaWebApiOptions options)
        {
            var issuer = UrlHelper.CreateIssuerUrl(options.OrgUrl, options.AuthorizationServerId);

            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
              issuer + "/.well-known/openid-configuration",
              new OpenIdConnectConfigurationRetriever(),
              new HttpDocumentRetriever());

            var signingKeyProvider = new DiscoveryDocumentSigningKeyProvider(configurationManager);

            var tokenValidationParameters = new DefaultTokenValidationParameters(options, issuer)
            {
                ValidAudience = options.Audience,
                IssuerSigningKeyResolver = (token, securityToken, keyId, validationParameters) =>
                {
                    var signingKeys = signingKeyProvider.GetSigningKeysAsync().GetAwaiter().GetResult();
                    return signingKeys.Where(x => x.KeyId == keyId);
                }
            };

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = tokenValidationParameters,
                TokenHandler = new StrictTokenHandler() { ClientId = options.ClientId }
            });
        }

        private static void AddOpenIdConnectAuthentication(IAppBuilder app, OktaMvcOptions options)
        {
            var issuer = UrlHelper.CreateIssuerUrl(options.OrgUrl, options.AuthorizationServerId);
            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
              issuer + "/.well-known/openid-configuration",
              new OpenIdConnectConfigurationRetriever(),
              new HttpDocumentRetriever());

            var tokenValidationParameters = new DefaultTokenValidationParameters(options, issuer)
            {
                NameClaimType = "name"
            };

            var tokenExchanger = new TokenExchanger(options, issuer, configurationManager);

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret,
                Authority = issuer,
                RedirectUri = options.RedirectUri,
                ResponseType = OpenIdConnectResponseType.CodeIdToken,
                Scope = options.Scope,
                PostLogoutRedirectUri = options.PostLogoutRedirectUri,
                TokenValidationParameters = tokenValidationParameters,

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthorizationCodeReceived = tokenExchanger.ExchangeCodeForToken,
                    RedirectToIdentityProvider = BeforeRedirectToIdentityProvider,
                },
            });
        }

        private static Task BeforeRedirectToIdentityProvider(RedirectToIdentityProviderNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> n)
        {
            // If signing out, add the id_token_hint
            if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
            {
                var idTokenClaim = n.OwinContext.Authentication.User.FindFirst("id_token");

                if (idTokenClaim != null)
                {
                    n.ProtocolMessage.IdTokenHint = idTokenClaim.Value;
                }

            }

            return Task.CompletedTask;
        }
    }
}
