using Owin;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Security.Claims;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using Okta.AspNet.Abstractions;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security;

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

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            SetupOpenIdConnectAuthentication(app, options);

            return app;
        }

        public static IAppBuilder UseOktaWebApi(this IAppBuilder app, OktaWebApiOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
               options.OrgUrl + "/.well-known/openid-configuration",
              new OpenIdConnectConfigurationRetriever(),
              new HttpDocumentRetriever());

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = options.Audience,
                    ValidIssuer = options.OrgUrl,
                    IssuerSigningKeyResolver = (token, securityToken, identifier, parameters) =>
                    {
                        var discoveryDocument = Task.Run(() => configurationManager.GetConfigurationAsync()).GetAwaiter().GetResult();
                        return discoveryDocument.SigningKeys;
                    }
                }
            });

            return app;
        }

        private static void SetupOpenIdConnectAuthentication(IAppBuilder app, OktaMvcOptions options)
        {
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret,
                Authority = options.OrgUrl,
                RedirectUri = options.RedirectUri,
                ResponseType = OpenIdConnectResponseType.CodeIdToken,
                Scope = (string.IsNullOrEmpty(options.Scope) ? OpenIdConnectScope.OpenIdProfile : options.Scope),
                PostLogoutRedirectUri = options.PostLogoutRedirectUri,
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name"
                },

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthorizationCodeReceived = async n =>
                    {
                        // Exchange code for access and ID tokens
                        var tokenClient = new TokenClient(options.OrgUrl + "/v1/token", options.ClientId, options.ClientSecret);
                        var tokenResponse = await tokenClient.RequestAuthorizationCodeAsync(n.Code, options.RedirectUri);

                        if (tokenResponse.IsError)
                        {
                            throw new Exception(tokenResponse.Error);
                        }

                        var userInfoClient = new UserInfoClient(options.OrgUrl + "/v1/userinfo");
                        var userInfoResponse = await userInfoClient.GetAsync(tokenResponse.AccessToken);
                        var claims = new List<Claim>();
                        claims.AddRange(userInfoResponse.Claims);
                        claims.Add(new Claim("id_token", tokenResponse.IdentityToken));
                        claims.Add(new Claim("access_token", tokenResponse.AccessToken));

                        if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
                        {
                            claims.Add(new Claim("refresh_token", tokenResponse.RefreshToken));
                        }

                        n.AuthenticationTicket.Identity.AddClaims(claims);

                        return;
                    },

                    RedirectToIdentityProvider = n =>
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
                },
            });
        }
    }
}