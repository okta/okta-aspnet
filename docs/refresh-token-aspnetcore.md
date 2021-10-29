

## Refreshing Access Token in ASP.NET CORE

Refreshing tokens can be done in different ways and samples are available across the Internet. Here is a working sample from this article [Handling Expired Refresh Tokens in ASP.NET Core](https://newbedev.com/handling-expired-refresh-tokens-in-asp-net-core), simplified and adapted to be used with Okta middleware. 

### ConfigureServices

```csharp
//..............................................................
           services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
            })
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(options=>
            {
               options.Events.OnValidatePrincipal += OnValidatePrincipal;
            })
           .AddOktaMvc(new OktaMvcOptions
           {
               GetClaimsFromUserInfoEndpoint = true,
               // Replace these values with your Okta configuration
               OktaDomain = Configuration.GetValue<string>("Okta:OktaDomain"),
               AuthorizationServerId = Configuration.GetValue<string>("Okta:AuthorizationServerId"),
               ClientId = Configuration.GetValue<string>("Okta:ClientId"),
               ClientSecret = Configuration.GetValue<string>("Okta:ClientSecret"),
               Scope = new List<string> { "openid", "profile", "email" , "offline_access" },
           });
//..............................................................
```



### OnValidatePrincipal handler

```csharp
        private async Task OnValidatePrincipal(CookieValidatePrincipalContext context)
        {
            const string accessTokenName = "access_token";
            const string refreshTokenName = "refresh_token";
            const string expirationTokenName = "expires_at";

            if (context.Principal.Identity.IsAuthenticated)
            {
                var exp = context.Properties.GetTokenValue(expirationTokenName);
                if (exp != null)
                {
                    var expires = DateTime.Parse(exp, CultureInfo.InvariantCulture).ToUniversalTime();
                    if (expires < DateTime.UtcNow)
                    {
                        var refreshToken = context.Properties.GetTokenValue(refreshTokenName);
                        if (refreshToken == null)
                        {
                            context.RejectPrincipal();
                            return;
                        }

                        var cancellationToken = context.HttpContext.RequestAborted;
                        var tokenResponse = await RequestNewToken(context, refreshToken, cancellationToken);
                        if (tokenResponse.IsError)
                        {
                            context.RejectPrincipal();
                            return;
                        }

                        // Update the tokens
                        var expirationValue = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn).ToString("o", CultureInfo.InvariantCulture);
                        context.Properties.StoreTokens(new[]
                        {
                            new AuthenticationToken { Name = refreshTokenName, Value = tokenResponse.RefreshToken },
                            new AuthenticationToken { Name = accessTokenName, Value = tokenResponse.AccessToken },
                            new AuthenticationToken { Name = expirationTokenName, Value = expirationValue }
                        });

                        // Update the cookie with the new tokens
                        context.ShouldRenew = true;
                    }
                }
            }
        }

        private async Task<TokenResponse> RequestNewToken(CookieValidatePrincipalContext context, string refreshToken, CancellationToken cancellationToken)
        {
            using var httpClient = new HttpClient();

            var oktaDomain = Configuration.GetValue<string>("Okta:OktaDomain");
            var authorizationServerId = Configuration.GetValue<string>("Okta:AuthorizationServerId");

            var tokenClientOptions = new TokenClientOptions
            {
                Address = $"{oktaDomain}/oauth2/{authorizationServerId}/v1/token",
                ClientId = Configuration.GetValue<string>("Okta:ClientId"),
                ClientSecret = Configuration.GetValue<string>("Okta:ClientSecret"),
            };

            var tokenClient = new TokenClient(httpClient, tokenClientOptions);
            var tokenResponse = await tokenClient.RequestRefreshTokenAsync(refreshToken, cancellationToken: cancellationToken).ConfigureAwait(false);
            
            return tokenResponse;
        }
```
