[<img src="https://devforum.okta.com/uploads/oktadev/original/1X/bf54a16b5fda189e4ad2706fb57cbb7a1e5b8deb.png" align="right" width="256px"/>](https://devforum.okta.com/)

[![Support](https://img.shields.io/badge/support-Developer%20Forum-blue.svg)](https://devforum.okta.com/)

## Installing the package

You can install the package by using the NuGet Package Explorer to search for [Okta.AspNetCore](https://nuget.org/packages/Okta.AspNetCore).

Or, you can use the `dotnet` command:

```
dotnet add package Okta.AspNetCore
```

# Usage example

Okta plugs into your OWIN Startup class with the `UseOktaWebApi()` method:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = OktaDefaults.ApiAuthenticationScheme;
        options.DefaultChallengeScheme = OktaDefaults.ApiAuthenticationScheme;
        options.DefaultSignInScheme = OktaDefaults.ApiAuthenticationScheme;
    })
    .AddOktaWebApi(new OktaWebApiOptions()
    {
        ClientId = Configuration["Okta:ClientId"],
        OktaDomain = Configuration["Okta:OktaDomain"],
        AuthorizationServerId = Configuration["Okta:AuthorizationServerId"]
    });

    services.AddMvc();
}
```

## That's it!

Placing the `[Authorize]` attribute on your controllers or actions will require a valid access token for those routes. This package will [parse and validate the access token](https://developer.okta.com/blog/2017/06/21/what-the-heck-is-oauth#oauth-flows) and populate `Http.Context` with a limited set of user information.

## Configuration Reference

The `OktaWebApiOptions` class configures the Okta middleware. You can see all the available options in the table below:

| Property                  | Required?    | Details                         |
|---------------------------|--------------|---------------------------------|
| OktaDomain                    | **Yes**      | Your Okta domain, i.e https://dev-123456.oktapreview.com  | 
| ClientId                  | **Yes**      | The client ID of your Okta Application |
| AuthorizationServerId     | No           | The Okta Authorization Server to use. The default value is `default`. |
| Audience                  | No           | The expected audience of incoming tokens. The default value is `api://default`. |
| ClockSkew                 | No           | The clock skew allowed when validating tokens. The default value is 2 minutes. |

You can store these values in the `appsettings.json`.
