

## About Refreshing Access Token in ASP.NET CORE

Refreshing access tokens in Asp.Net Core can be done in various ways and samples are available across the Internet. For example this can be done in the OnValidatePrincipal event handler and attached like in here:

```
    .AddCookie(options=>
    {
       options.Events.OnValidatePrincipal += OnValidatePrincipalHandler;
    })
```

Another option is to do it in a custom middleware as for Asp.Net Framework.

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
