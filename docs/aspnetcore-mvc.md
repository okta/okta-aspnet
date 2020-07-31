[<img src="https://devforum.okta.com/uploads/oktadev/original/1X/bf54a16b5fda189e4ad2706fb57cbb7a1e5b8deb.png" align="right" width="256px"/>](https://devforum.okta.com/)

[![Support](https://img.shields.io/badge/support-Developer%20Forum-blue.svg)](https://devforum.okta.com/)

# Installing the package

You can install the package by using the NuGet Package Explorer to search for [Okta.AspNetCore](https://nuget.org/packages/Okta.AspNetCore).

Or, you can use the `dotnet` command:

```
dotnet add package Okta.AspNetCore
```

# Usage guide

These examples will help you to understand how to use this library. You can also check out our [ASP.NET Core samples](https://github.com/okta/samples-aspnetcore).

## Basic configuration

Okta plugs into your OWIN Startup class with the `UseOktaMvc()` method:

```csharp

public void ConfigureServices(IServiceCollection services)
{
    var oktaMvcOptions = new OktaMvcOptions()
    {
        OktaDomain = Configuration.GetSection("Okta").GetValue<string>("OktaDomain"),
        ClientId = Configuration.GetSection("Okta").GetValue<string>("ClientId"),
        ClientSecret = Configuration.GetSection("Okta").GetValue<string>("ClientSecret"),
        Scope = new List<string> { "openid", "profile", "email" },
    };

    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OktaDefaults.MvcAuthenticationScheme;
    })
    .AddCookie()
    .AddOktaMvc(oktaMvcOptions);

    services.AddMvc();
}
```

### That's it!

Placing the `[Authorize]` attribute on your controllers or actions will check whether the user is logged in, and redirect them to Okta if necessary.

ASP.NET automatically populates `HttpContext.User` with the information Okta sends back about the user. You can check whether the user is logged in with `User.Identity.IsAuthenticated` in your actions or views.

## Proxy configuration

If your application requires proxy server settings, specify the `Proxy` property on `OktaMvcOptions`.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    var oktaMvcOptions = new OktaMvcOptions()
    {
        OktaDomain = Configuration.GetSection("Okta").GetValue<string>("OktaDomain"),
        ClientId = Configuration.GetSection("Okta").GetValue<string>("ClientId"),
        ClientSecret = Configuration.GetSection("Okta").GetValue<string>("ClientSecret"),
        Scope = new List<string> { "openid", "profile", "email" },
        Proxy = new ProxyConfiguration
        {
            Host = "http://{yourProxyHostNameOrIp}",
            Port = 3128, // Replace this value with the port that your proxy server listens on
            Username = "{yourProxyServerUserName}",
            Password = "{yourProxyServerPassword}",
        }
    };

    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OktaDefaults.MvcAuthenticationScheme;
    })
    .AddCookie()
    .AddOktaMvc(oktaMvcOptions);

    services.AddMvc();
}
```

## Self-Hosted login configuration

```csharp
public void ConfigureServices(IServiceCollection services)
{
    var oktaMvcOptions = new OktaMvcOptions()
    {
        OktaDomain = Configuration.GetSection("Okta").GetValue<string>("OktaDomain"),
        ClientId = Configuration.GetSection("Okta").GetValue<string>("ClientId"),
        ClientSecret = Configuration.GetSection("Okta").GetValue<string>("ClientSecret"),
        Scope = new List<string> { "openid", "profile", "email" },
    };

    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = new PathString("/Account/SignIn");
    })
    .AddOktaMvc(oktaMvcOptions);

    services.AddMvc();
}
```
> Note: If you are using role-based authorization and you need to redirect unauthorized users to an access-denied page or similar, check out [CookieAuthenticationOptions.AccessDeniedPath](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.cookies.cookieauthenticationoptions.accessdeniedpath?view=aspnetcore-2.2).

## Login with an external identity provider

Add the following action in your controller: 

```csharp
public IActionResult SignInWithIdp(string idp)
{
    if (!HttpContext.User.Identity.IsAuthenticated)
    {
        var properties = new AuthenticationProperties();
        properties.Items.Add("idp", idp);
        properties.RedirectUri = "/Home/";

        return Challenge(properties, OktaDefaults.MvcAuthenticationScheme);
    }

    return RedirectToAction("Index", "Home");
}
```

## Accessing OIDC Tokens

To access OIDC tokens, AspNet Core provides the [`HttpContext.GetTokenAsync(...)`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.authenticationtokenextensions.gettokenasync) extension method.  The following is an example of how to access OIDC tokens from your `HomeController`:

```csharp
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class TokenModel
{
    public string Name { get; set; }

    public string Value { get; set; }
}

public class HomeController : Controller
{
    [Authorize]
    public async Task<ActionResult> OIDCToken(string tokenName)
    {
        var tokenValue = await HttpContext.GetTokenAsync(tokenName);
        return View(new TokenModel { Name = tokenName, Value = tokenValue });
    }
}
```

This example assumes you have a view called `OIDCToken` whose model is of type `TokenModel`. The OIDC tokens are `id_token` and `access_token` as well as `refresh_token` if available.

## Handling failures

In the event a failure occurs, the Okta.AspNetCore library provides the `OnOktaApiFailure` and `OnAuthenticationFailed` delegates defined on the `OktaMvcOptions` class. The following is an example of how to use `OnOktaApiFailure` and `OnAuthenticationFailed` to handle failures:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOktaMvc(new OktaMvcOptions
        {
            // ... other configuration options removed for brevity ...
            OnOktaApiFailure = OnOktaApiFailure,
            OnAuthenticationFailed = OnAuthenticationFailed,
        });
    }

    public async Task OnOktaApiFailure(RemoteFailureContext context)
    {
        await Task.Run(() =>
        {
            context.Response.Redirect("{YOUR-EXCEPTION-HANDLING-ENDPOINT}?message=" + context.Failure.Message);
            context.HandleResponse();
        });
    }

    public async Task OnAuthenticationFailed(AuthenticationFailedContext context)
    {
        await Task.Run(() =>
        {
            context.Response.Redirect("{YOUR-EXCEPTION-HANDLING-ENDPOINT}?message=" + context.Exception.Message);
            context.HandleResponse();
        });
    }
}
```

The Okta.AspNetCore library will include your identity provider id in the authorize URL and the user will prompted with the identity provider login. For more information, check out our guides to [add an external identity provider](https://developer.okta.com/docs/guides/add-an-external-idp/).

# Configuration Reference 

The `OktaMvcOptions` class configures the Okta middleware. You can see all the available options in the table below:


| Property                  | Required?    | Details                         |
|---------------------------|--------------|---------------------------------|
| OktaDomain                    | **Yes**      | Your Okta domain, i.e https://dev-123456.oktapreview.com  | 
| ClientId                  | **Yes**      | The client ID of your Okta Application |
| ClientSecret              | **Yes**      | The client secret of your Okta Application |
| CallbackPath               | **Yes**      | The location Okta should redirect to process a login. This is typically `http://{yourApp}/authorization-code/callback`. No matter the value, the redirect is handled automatically by this package, so you don't need to write any custom code to handle this route. |
| AuthorizationServerId     | No           | The Okta Authorization Server to use. The default value is `default`. |
| PostLogoutRedirectUri     | No           | The location Okta should redirect to after logout. If blank, Okta will redirect to the Okta login page. |
| Scope                     | No           | The OAuth 2.0/OpenID Connect scopes to request when logging in. The default value is `openid profile`. |
| GetClaimsFromUserInfoEndpoint | No       | Whether to retrieve additional claims from the UserInfo endpoint after login. The default value is `true`. |
| ClockSkew                 | No           | The clock skew allowed when validating tokens. The default value is 2 minutes. |
| OnTokenValidated                 | No           | The event invoked after the security token has passed validation and a ClaimsIdentity has been generated. |
| OnUserInformationReceived                 | No           | The event invoked when user information is retrieved from the UserInfoEndpoint. The `GetClaimsFromUserInfoEndpoint` value must be `true` when using this event. |
| OnOktaApiFailure          | No           | The event invoked when a failure occurs within the Okta API. |
| OnAuthenticationFailed    | No           | The event invoked if exceptions are thrown during request processing. |
| Proxy                     | No           | An object describing proxy server configuration.  Properties are `Host`, `Port`, `Username` and `Password` |

You can store these values (except the events) in the `appsettings.json`, but be careful when checking in the client secret to the source control.

> Note: You can use the [The Org Authorization Server](https://developer.okta.com/docs/concepts/auth-servers/#org-authorization-server) for common use cases such as adding authentication to your MVC Application or checking user's profile, but the access token issued by this Authorization Server cannot be used or validated by your own applications.  Check out the [Okta documentation](https://developer.okta.com/docs/concepts/auth-servers/#org-authorization-server) to learn more.
