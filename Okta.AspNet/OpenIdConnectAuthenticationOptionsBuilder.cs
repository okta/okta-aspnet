// <copyright file="OpenIdConnectAuthenticationOptionsBuilder.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Okta.AspNet.Abstractions;

namespace Okta.AspNet
{
    public class OpenIdConnectAuthenticationOptionsBuilder
    {
        private readonly OktaMvcOptions _oktaMvcOptions;
        private readonly IUserInformationProvider _userInformationProvider;
        private readonly string _issuer;
        private readonly ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
        private readonly HttpClient _httpClient;

        public OpenIdConnectAuthenticationOptionsBuilder(OktaMvcOptions oktaMvcOptions, IUserInformationProvider userInformationProvider = null)
        {
            _oktaMvcOptions = oktaMvcOptions;
            _issuer = UrlHelper.CreateIssuerUrl(oktaMvcOptions.OktaDomain, oktaMvcOptions.AuthorizationServerId);
            _httpClient = new HttpClient(new OktaHttpMessageHandler("okta-aspnet", typeof(OktaMiddlewareExtensions).Assembly.GetName().Version, oktaMvcOptions));
            _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    _issuer + "/.well-known/openid-configuration",
                    new OpenIdConnectConfigurationRetriever(),
                    new HttpDocumentRetriever(_httpClient));

            _userInformationProvider = userInformationProvider ?? new UserInformationProvider(oktaMvcOptions, _issuer, _configurationManager);
        }

        /// <summary>
        /// Creates a new instance of OpenIdConnectAuthenticationOptions.
        /// </summary>
        /// <param name="oktaMvcOptions">The <see cref="OktaMvcOptions"/> options.</param>
        /// <returns>A new instance of OpenIdConnectAuthenticationOptions.</returns>
        public OpenIdConnectAuthenticationOptions BuildOpenIdConnectAuthenticationOptions()
        {
            var tokenValidationParameters = new DefaultTokenValidationParameters(_oktaMvcOptions, _issuer)
            {
                NameClaimType = "name",
                ValidAudience = _oktaMvcOptions.ClientId,
            };

            var userInfoProvider = new UserInformationProvider(_oktaMvcOptions, _issuer, _configurationManager);
            var definedScopes = _oktaMvcOptions.Scope?.ToArray() ?? OktaDefaults.Scope;
            var scopeString = string.Join(" ", definedScopes);

            var oidcOptions = new OpenIdConnectAuthenticationOptions
            {
                ClientId = _oktaMvcOptions.ClientId,
                ClientSecret = _oktaMvcOptions.ClientSecret,
                Authority = _issuer,
                RedirectUri = _oktaMvcOptions.RedirectUri,
                ResponseType = OpenIdConnectResponseType.Code,
                RedeemCode = true,
                Scope = scopeString,
                PostLogoutRedirectUri = _oktaMvcOptions.PostLogoutRedirectUri,
                TokenValidationParameters = tokenValidationParameters,
                SecurityTokenValidator = new StrictSecurityTokenValidator(),
                AuthenticationMode = (_oktaMvcOptions.LoginMode == LoginMode.SelfHosted) ? AuthenticationMode.Passive : AuthenticationMode.Active,
                SaveTokens = true,
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    RedirectToIdentityProvider = BeforeRedirectToIdentityProviderAsync,
                    SecurityTokenValidated = SecurityTokenValidatedAsync,
                    AuthenticationFailed = _oktaMvcOptions.AuthenticationFailed,
                },
            };

            return oidcOptions;
        }

        private Task BeforeRedirectToIdentityProviderAsync(RedirectToIdentityProviderNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> redirectToIdentityProviderNotification)
        {
            // If signing out, add the id_token_hint
            if (redirectToIdentityProviderNotification.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
            {
                if (redirectToIdentityProviderNotification.OwinContext.Authentication.User.FindFirst("id_token") != null)
                {
                    redirectToIdentityProviderNotification.ProtocolMessage.IdTokenHint = redirectToIdentityProviderNotification.OwinContext.Authentication.User.FindFirst("id_token").Value;
                }
            }

            // Verify if additional well-known params (e.g login-hint, sessionToken, idp, etc.) should be sent in the request.
            if (redirectToIdentityProviderNotification.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication)
            {
                var oktaRequestParamValue = string.Empty;

                foreach (var oktaParamKey in OktaParams.AllParams)
                {
                    redirectToIdentityProviderNotification.OwinContext.Authentication.AuthenticationResponseChallenge?.Properties?.Dictionary?.TryGetValue(oktaParamKey, out oktaRequestParamValue);

                    if (!string.IsNullOrEmpty(oktaRequestParamValue))
                    {
                        redirectToIdentityProviderNotification.ProtocolMessage.SetParameter(oktaParamKey, oktaRequestParamValue);
                    }
                }
            }

            return Task.FromResult(false);
        }

        private async Task SecurityTokenValidatedAsync(SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            context.AuthenticationTicket.Identity.AddClaim(new Claim("id_token", context.ProtocolMessage.IdToken));
            context.AuthenticationTicket.Identity.AddClaim(new Claim("access_token", context.ProtocolMessage.AccessToken));

            if (!string.IsNullOrEmpty(context.ProtocolMessage.RefreshToken))
            {
                context.AuthenticationTicket.Identity.AddClaim(new Claim("refresh_token", context.ProtocolMessage.RefreshToken));
            }

            FillNameIdentifierClaimOnIdentity(context.AuthenticationTicket.Identity);

            if (_oktaMvcOptions.GetClaimsFromUserInfoEndpoint)
            {
                await _userInformationProvider.EnrichIdentityViaUserInfoAsync(context.AuthenticationTicket.Identity, context.ProtocolMessage.AccessToken).ConfigureAwait(false);
            }

            await _oktaMvcOptions.SecurityTokenValidated(context).ConfigureAwait(false);
        }

        /// <summary>
        /// For compatibility with the .NET MVC antiforgery provider, make sure the old-style NameIdentifier is filled.
        /// If not, get subject claim and duplicate it to MSFT's NameIdentifier.
        /// </summary>
        /// <param name="identity">The <see cref="ClaimsIdentity"/> to modify in place.</param>
        private void FillNameIdentifierClaimOnIdentity(ClaimsIdentity identity)
        {
            var currentNameIdentifier = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var sub = identity.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            if (currentNameIdentifier == null && sub != null)
            {
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, sub));
            }
        }
    }
}
