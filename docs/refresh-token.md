

## Refreshing Access Token in ASP.NET MVC or WebForms App

The following is an example of middleware that handles the token refresh process based on the sample provided by [@Good-man](https://github.com/Good-man) on [this issue](https://github.com/okta/okta-aspnet/issues/130).

### Startup.cs

App configuration code may look like this. Note that Scope includes ```offline_access```. That is required for refresh tokens to work.

```csharp
        public void Configuration(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            
            // Adding the Refresh token middleware
            var oktaDomain = ConfigurationManager.AppSettings["okta:OktaDomain"];
            var authServerId = ConfigurationManager.AppSettings["okta:AuthorizationServerId"];
            var tokenEndpoint = $"{oktaDomain}/oauth2/{authServerId}/v1/token";

            app.Use(typeof(RefreshTokenMiddleware), new RefreshTokenMiddlewareOptions
            {
                TokenEndpoint = tokenEndpoint,
                ClientId = ConfigurationManager.AppSettings["okta:ClientId"],
                ClientSecret = ConfigurationManager.AppSettings["okta:ClientSecret"],
            });
            
            // Adding Okta Mvc
            app.UseOktaMvc(new OktaMvcOptions()
            {
                OktaDomain = oktaDomain,
                ClientId = ConfigurationManager.AppSettings["okta:ClientId"],
                ClientSecret = ConfigurationManager.AppSettings["okta:ClientSecret"],
                AuthorizationServerId = authServerId,
                RedirectUri = ConfigurationManager.AppSettings["okta:RedirectUri"],
                PostLogoutRedirectUri = ConfigurationManager.AppSettings["okta:PostLogoutRedirectUri"],
                GetClaimsFromUserInfoEndpoint = true,

                Scope = new List<string> { "openid", "profile", "email", "offline_access" },
            });
        }
```



### RefreshTokenMiddleware.cs

Refresh token logic is implemented in this class which can be added to the OWIN Middleware Pipeline.

```csharp
    public class RefreshTokenMiddleware : OwinMiddleware
    {
        private readonly RefreshTokenMiddlewareOptions _options;

        public RefreshTokenMiddleware(OwinMiddleware next, RefreshTokenMiddlewareOptions options) : base(next)
        {
            _options = options;
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (AccessTokenExpired(context) && HasRefreshToken(context))
            {
                await RefreshTokens(context);
            }

            await Next.Invoke(context);
        }

        private bool HasRefreshToken(IOwinContext context)
        {
            if (context.Authentication.User.Identity is ClaimsIdentity identity && identity.IsAuthenticated)
            {
                return identity.Claims.Any(c => c.Type == ClaimTypeKey.RefreshToken);
            }

            return false;
        }

        private bool AccessTokenExpired(IOwinContext context)
        {
            if (context.Authentication.User?.Identity is ClaimsIdentity identity && identity.IsAuthenticated)
            {
                var accessTokenString = identity.FindFirst(ClaimTypeKey.AccessToken)?.Value;
                if (accessTokenString == null)
                {
                    return false;
                }

                var accessToken = new StrictTokenHandler().ReadToken(accessTokenString);

                return accessToken.ValidTo <= DateTime.UtcNow;
            }

            return false;
        }

        private async Task RefreshTokens(IOwinContext context)
        {
            var identity = (ClaimsIdentity)context.Authentication.User.Identity;
            var refreshTokenClaim = identity.FindFirst(ClaimTypeKey.RefreshToken);
            if (refreshTokenClaim != null)
            {
                var iat = identity.FindFirst(JwtClaimTypes.IssuedAt);
                var exp = identity.FindFirst(JwtClaimTypes.Expiration);
                var accessTokenClaim = identity.FindFirst(ClaimTypeKey.AccessToken);
                var idTokenClaim = identity.FindFirst(ClaimTypeKey.IdToken);

                identity.RemoveClaim(iat);
                identity.RemoveClaim(exp);
                identity.RemoveClaim(refreshTokenClaim);
                identity.RemoveClaim(accessTokenClaim);
                identity.RemoveClaim(idTokenClaim);

                using (var httpClient = new HttpClient())
                {
                    var request = new RefreshTokenRequest
                    {
                        RefreshToken = refreshTokenClaim.Value,
                        ClientId = _options.ClientId,
                        ClientSecret = _options.ClientSecret,
                        Address = _options.TokenEndpoint,
                    };
                    var tokenResponse = await httpClient.RequestRefreshTokenAsync(request);

                    if (tokenResponse.IsError)
                    {
                        throw new SecurityTokenException(tokenResponse.Error);
                    }

                    var tokenHandler = new JwtSecurityToken(tokenResponse.AccessToken);

                    identity.AddClaims(new[] {
                            new Claim(ClaimTypeKey.AccessToken, tokenResponse.AccessToken),
                            new Claim(ClaimTypeKey.IdToken, tokenResponse.IdentityToken),
                            new Claim(ClaimTypeKey.RefreshToken, tokenResponse.RefreshToken),
                            new Claim(JwtClaimTypes.IssuedAt, tokenHandler.Payload.Iat?.ToString()),
                            new Claim(JwtClaimTypes.Expiration, tokenHandler.Payload.Exp?.ToString()),
                        });

                    context.Authentication.SignIn(new AuthenticationProperties
                    {
                        IssuedUtc = tokenHandler.IssuedAt,
                        ExpiresUtc = tokenHandler.ValidTo,
                    },
                        identity);
                }
            }
        }
	}	
```


### ClaimTypeKey.cs

Constants used by the Refresh Token Middleware.

```csharp
    internal static class ClaimTypeKey
    {
        internal const string RefreshToken = "refresh_token";
        internal const string AccessToken = "access_token";
        internal const string IdToken = "id_token";
    }
```
