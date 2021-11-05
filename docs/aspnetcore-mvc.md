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


ASP.NET Core Identity framework uses "Identity.Application" authentication scheme. Here is how configuration code looks like in such case:

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
        options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme; 
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


## Configuration for cloud services or load balancers

If you plan to deploy your application to a cloud service, you may need to do additional customization. Cloud services usually create similar environments in which applications run inside a Docker container using HTTP and behind a reverse proxy or load balancer. Cloud environment may comprise API gateway, firewall, load balancer, reverse proxy. This environment may be initially configured to use HTTP for incoming requests.

Although cloud services configurations may vary, general rules are:

* Make sure the environment is using HTTPS for incoming requests. 

This may be the default setting or may require some amount of fiddling as in case of AWS.


* Application should be configured to consider forwarded headers information. 

Backend application typically listens on HTTP and it sits behind a reverse proxy or load balancer that accepts user requests over HTTPS. If this isn't taken into account `redirect_uri` authentication parameter will not be built correctly as the function used for that [Microsoft.AspNetCore.Authentication.BuildRedirectUri](https://github.com/aspnet/Security/blob/26d27d871b7992022c082dc207e3d126e1d9d278/src/Microsoft.AspNetCore.Authentication/AuthenticationHandler.cs#L117) relies on request scheme. 

For information on how to configure your application in a proxied, load balanced environment, see [Configure ASP.NET Core to work with proxy servers and load balancers](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer).

Azure environment with [Front Door load balancer](https://docs.microsoft.com/en-us/azure/frontdoor/) should work properly after forwarded headers configuration is added like so:

`Startup.ConfigureServices:`
```csharp
     services.Configure<ForwardedHeadersOptions>(options =>
     {
         options.ForwardedHeaders =
             ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto;
     });
```

Use code similar to the following to properly configure a load balanced `AWS` or `Google Cloud with App Engine Flex` environment.

`Startup.Configure:`
```csharp
     app.Use((context, next) =>
     {
         var xProtoHeaders = context.Request.Headers["X-Forwarded-Proto"];
         if (xProtoHeaders.Count > 0 && xProtoHeaders[0].ToLower() == "https")
             context.Request.Scheme = "https";
         return next();
     });
```


Don't use `app.UseHttpsRedirection();` as it may cause a client infinite redirection loop.

* Do additional configuration if needed. 

Because of Docker specifics on some cloud services authentication may fail with the message `Unable to unprotect the message.State.` This most likely means you need to configure Data Protection storage. For Google Cloud, see [ASP.Net Core data protection provider](https://cloud.google.com/appengine/docs/flexible/dotnet/application-security#aspnet_core_data_protection_provider).



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

## Specifying the `login_hint` parameter

The `login_hint` parameter allows you to pass a username to prepopulate when prompting for authentication. For more details check out the [Okta documentation](https://developer.okta.com/docs/reference/api/oidc/#request-parameters).

Add the following action in your controller: 

```csharp
public IActionResult SignIn()
{
    if (!HttpContext.User.Identity.IsAuthenticated)
    {
        var properties = new AuthenticationProperties();
        properties.Items.Add(OktaParams.LoginHint, "darth.vader@imperial-senate.gov");
        properties.RedirectUri = "/Home/";

        return Challenge(properties, OktaDefaults.MvcAuthenticationScheme);
    }

    return RedirectToAction("Index", "Home");
}
```

## Login with an external identity provider

Add the following action in your controller: 

```csharp
public IActionResult SignInWithIdp(string idp)
{
    if (!HttpContext.User.Identity.IsAuthenticated)
    {
        var properties = new AuthenticationProperties();
        properties.Items.Add(OktaParams.Idp, idp);
        properties.RedirectUri = "/Home/";

        return Challenge(properties, OktaDefaults.MvcAuthenticationScheme);
    }

    return RedirectToAction("Index", "Home");
}
```
The Okta.AspNetCore library will include your identity provider id in the authorize URL and the user will prompted with the identity provider login. For more information, check out our guides to [add an external identity provider](https://developer.okta.com/docs/guides/add-an-external-idp/).

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

In the event a failure occurs, the Okta.AspNetCore library exposes [OpenIdConnectEvents](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.openidconnect.openidconnectevents.onauthenticationfailed) so you can hook into specific events during the authentication process. For more information See [`OnAuthenticationFailed`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.openidconnect.openidconnectevents.onauthenticationfailed) or [`OnRemoteFailure`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.remoteauthenticationevents.onremotefailure).


 The following is an example of how to use events to handle failures:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOktaMvc(new OktaMvcOptions
        {
            // ... other configuration options removed for brevity ...
            OpenIdConnectEvents = new OpenIdConnectEvents
            {
                OnAuthenticationFailed = OnAuthenticationFailed,
                OnRemoteFailure = OnOktaApiFailure,
            },
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
# Configuration Reference 

The `OktaMvcOptions` class configures the Okta middleware. You can see all the available options in the table below:


| Property                  | Required?    | Details                         |
|---------------------------|--------------|---------------------------------|
| OktaDomain                    | **Yes**      | Your Okta domain, i.e https://dev-123456.oktapreview.com  | 
| ClientId                  | **Yes**      | The client ID of your Okta Application |
| ClientSecret              | **Yes**      | The client secret of your Okta Application |
| CallbackPath               | **Yes**      | The location Okta should redirect to process a login. This is typically `http://{yourApp}/authorization-code/callback`. No matter the value, the redirect is handled automatically by this package, so you don't need to write any custom code to handle this route. |
| AuthorizationServerId     | No           | The Okta Authorization Server to use. The default value is `default`. Use `string.Empty` if you are using the Org Authorization Server. |
| PostLogoutRedirectUri     | No           | The location Okta should redirect to after logout. If blank, Okta will redirect to the Okta login page. |
| Scope                     | No           | The OAuth 2.0/OpenID Connect scopes to request when logging in. The default value is `openid profile`. |
| GetClaimsFromUserInfoEndpoint | No       | Whether to retrieve additional claims from the UserInfo endpoint after login. The default value is `true`. |
| ClockSkew                 | No           | The clock skew allowed when validating tokens. The default value is 2 minutes. |
| Proxy                     | No           | An object describing proxy server configuration.  Properties are `Host`, `Port`, `Username` and `Password` |
|OpenIdConnectEvents | No |  Specifies the [events](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.remoteauthenticationevents.onremotefailure) which the underlying OpenIdConnectHandler invokes to enable developer control over the authentication process.|

You can store these values (except the events) in the `appsettings.json`, but be careful when checking in the client secret to the source control.

> Note: You can use the [The Org Authorization Server](https://developer.okta.com/docs/concepts/auth-servers/#org-authorization-server) for common use cases such as adding authentication to your MVC Application or checking user's profile, but the access token issued by this Authorization Server cannot be used or validated by your own applications.  Check out the [Okta documentation](https://developer.okta.com/docs/concepts/auth-servers/#org-authorization-server) to learn more.
