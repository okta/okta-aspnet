

## About Refreshing Access Token in ASP.NET CORE

The Asp.Net SDK doesn't currently have built-in support for refreshing tokens. Instead use the direct approach with the OAuth token endpoint.

### Refresh the token using the OAuth token endpoint

You can refresh access and ID tokens using the `/token` endpoint with the` grant_type` set to `refresh_token`. Before calling this endpoint, obtain the refresh token from the SDK and ensure that you have included `offline_access` as a scope in the SDK configuration. For further details on access token refresh with this endpoint, see [Use a refresh token](https://developer.okta.com/docs/guides/refresh-tokens/use-refresh-token/).

The following is an example of a token request method:


```csharp
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

There are several ways you can call the `RequestNewToken` method. You can either hook into OnValidatePrincipal event or define a new Middleware and call the refresh function after the existing access token expires:

```csharp
    .AddCookie(options=>
    {
       options.Events.OnValidatePrincipal += OnValidatePrincipalHandler;
    })
```

After new authentication tokens are refreshed they can be persisted in cookies like in the following sampe:

```csharp
    var expirationValue = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn).ToString("o", CultureInfo.InvariantCulture);
    context.Properties.StoreTokens(new []
    {
        new AuthenticationToken { Name = refreshTokenName, Value = tokenResponse.RefreshToken },
        new AuthenticationToken { Name = accessTokenName, Value = tokenResponse.AccessToken },
        new AuthenticationToken { Name = expirationTokenName, Value = expirationValue }
    });

    context.ShouldRenew = true;
```
