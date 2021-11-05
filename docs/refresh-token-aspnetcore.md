

## About Refreshing Access Token in ASP.NET CORE

Refreshing access tokens in Asp.Net Core can be done in various ways and samples are available across the Internet. For example this can be done in the OnValidatePrincipal event handler and attached like in here:

```
    .AddCookie(options=>
    {
       options.Events.OnValidatePrincipal += OnValidatePrincipalHandler;
    })
```

Another option is to implement refreshing token in a custom middleware.

Don't forget to include "offline_access" scope into scope list in `OktaMvcOptions` for minting refresh token.

```
   .AddOktaMvc(new OktaMvcOptions
   {
       GetClaimsFromUserInfoEndpoint = true,
       OktaDomain = Configuration.GetValue<string>("Okta:OktaDomain"),
       AuthorizationServerId = Configuration.GetValue<string>("Okta:AuthorizationServerId"),
       ClientId = Configuration.GetValue<string>("Okta:ClientId"),
       ClientSecret = Configuration.GetValue<string>("Okta:ClientSecret"),
       Scope = new List<string> { "openid", "profile", "email" , "offline_access" },
   });

```
