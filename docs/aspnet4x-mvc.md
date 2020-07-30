[<img src="https://devforum.okta.com/uploads/oktadev/original/1X/bf54a16b5fda189e4ad2706fb57cbb7a1e5b8deb.png" align="right" width="256px"/>](https://devforum.okta.com/)

[![Support](https://img.shields.io/badge/support-Developer%20Forum-blue.svg)](https://devforum.okta.com/)

# Installing the package

You can install the package by using the NuGet Package Explorer to search for [Okta.AspNet](https://nuget.org/packages/Okta.AspNet).

Or, you can use the `dotnet` command:


```
dotnet add package Okta.AspNet
```
# Usage guide

These examples will help you to understand how to use this library. You can also check out our ASP.NET samples:

* [ASP.NET MVC Samples](https://github.com/okta/samples-aspnet)
* [ASP.NET Web Forms Samples](https://github.com/okta/samples-aspnet-webforms)

## Basic configuration

Okta plugs into your OWIN Startup class with the `UseOktaMvc()` method:

```csharp
public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
        app.UseCookieAuthentication(new CookieAuthenticationOptions());

        app.UseOktaMvc(new OktaMvcOptions
        {
            OktaDomain = "https://{yourOktaDomain}",
            ClientId = "{clientId}",
            ClientSecret = "{clientSecret}",
            AuthorizationServerId = "default",
            RedirectUri = "http://localhost:8080/authorization-code/callback",
            PostLogoutRedirectUri = "http://localhost:8080/Home"
        });
    }
}
```

## Proxy configuration

If your application requires proxy server settings, specify the `Proxy` property on `OktaMvcOptions`.

```csharp
public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        app.UseOktaMvc(new OktaMvcOptions
        {
            // ... other configuration removed for brevity

            Proxy = new ProxyConfiguration
            {
                Host = "http://{yourProxyHostNameOrIp}",
                Port = 3128, // Replace this value with the port that your proxy server listens on
                Username = "{yourProxyServerUserName}",
                Password = "{yourProxyServerPassword}",
            }
        });
    }
}
```

### That's it!

Placing the `[Authorize]` attribute on your controllers or actions will check whether the user is logged in, and redirect them to Okta if necessary.

ASP.NET automatically populates `HttpContext.User` with the information Okta sends back about the user. You can check whether the user is logged in with `User.Identity.IsAuthenticated` in your actions or views.

## Self-Hosted login configuration

```csharp
public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
        app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                LoginPath = new PathString("/Account/Login"),
            });

        app.UseOktaMvc(new OktaMvcOptions
        {
            OktaDomain = "https://{yourOktaDomain}",
            ClientId = "{clientId}",
            ClientSecret = "{clientSecret}",
            AuthorizationServerId = "default",
            RedirectUri = "http://localhost:8080/authorization-code/callback",
            PostLogoutRedirectUri = "http://localhost:8080/Home",
            LoginMode = LoginMode.SelfHosted
        });
    }
}
```

> Note: If you are using role-based authorization and you need to redirect unauthorized users to an access-denied page or similar, check out [CookieAuthenticationProvider.ApplyRedirect](https://docs.microsoft.com/en-us/previous-versions/aspnet/mt152260(v%3Dvs.113)).

## Login with an external identity provider

Add the following action in your controller: 

```csharp
public ActionResult LoginWithIdp(string idp)
{
    if (!HttpContext.User.Identity.IsAuthenticated)
    {
        var properties = new AuthenticationProperties();
        properties.Dictionary.Add("idp", idp);
        properties.RedirectUri = "/Home/About";

        HttpContext.GetOwinContext().Authentication.Challenge(properties,
            OktaDefaults.MvcAuthenticationType);

        return new HttpUnauthorizedResult();
    }

    return RedirectToAction("Index", "Home");
}
```

The Okta.AspNet library will include your identity provider id in the authorize URL and the user will prompted with the identity provider login. For more information, check out our guides to [add an external identity provider](https://developer.okta.com/docs/guides/add-an-external-idp/).

## Accessing OIDC Tokens

For your convenience, the Okta.AspNet library makes OIDC tokens available as user claims accessible from a controller in your application.  The following is an example of how to access OIDC tokens from your `HomeController`:

```csharp
public class HomeController : Controller
{
    [Authorize]
    public async Task<ActionResult> Claim(string claimType)
    {
        var claim = HttpContext.GetOwinContext().Authentication.User.Claims.First(c => c.Type == claimType);
        return View(claim);
    } 
}
```

This example assumes you have a view called `Claim` whose model is of type `System.Security.Claims.Claim`. The claim types for OIDC tokens are `id_token` and `access_token` as well as `refresh_token` if available.

## Handling failures

In the event a failure occurs, the Okta.AspNet library provides the `OnAuthenticationFailed` delegate defined on the `OktaMvcOptions` class. The following is an example of how to use `OnAuthenticationFailed` to handle authentication failures:

```csharp
public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        app.UseOktaMvc(new OktaMvcOptions()
        {
            // ... other configuration options removed for brevity ...
            AuthenticationFailed = OnAuthenticationFailed,
        });
    }

    public async Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification)
    {
        await Task.Run(() =>
        {
            notification.Response.Redirect("{YOUR-EXCEPTION-HANDLING-ENDPOINT}?message=" + notification.Exception.Message);
            notification.HandleResponse();
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
| RedirectUri               | **Yes**      | The location Okta should redirect to process a login. This is typically `http://{yourApp}/authorization-code/callback`. No matter the value, the redirect is handled automatically by this package, so you don't need to write any custom code to handle this route. |
| AuthorizationServerId     | No           | The Okta Authorization Server to use. The default value is `default`. |
| PostLogoutRedirectUri     | No           | The location Okta should redirect to after logout. If blank, Okta will redirect to the Okta login page. |
| Scope                     | No           | The OAuth 2.0/OpenID Connect scopes to request when logging in. The default value is `openid profile`. |
| LoginMode                     | No           | LoginMode controls the login redirect behavior of the middleware. The default value is `OktaHosted`. |
| GetClaimsFromUserInfoEndpoint | No       | Whether to retrieve additional claims from the UserInfo endpoint after login. The default value is `true`. |
| ClockSkew                 | No           | The clock skew allowed when validating tokens. The default value is 2 minutes. |
| SecurityTokenValidated                 | No           | The event invoked after the security token has passed validation and a `ClaimsIdentity` has been generated. |
| OnAuthenticationFailed    | No           | The event invoked if exceptions are thrown during request processing. |

You can store these values (except the Token event) in the `Web.config`, but be careful when checking in the client secret to the source control.

> Note: You can use the [The Org Authorization Server](https://developer.okta.com/docs/concepts/auth-servers/#org-authorization-server) for common use cases such as adding authentication to your MVC Application or checking user's profile, but the access token issued by this Authorization Server cannot be used or validated by your own applications.  Check out the [Okta documentation](https://developer.okta.com/docs/concepts/auth-servers/#org-authorization-server) to learn more.

# Troubleshooting

If you are using .NET framework <4.6 or you are getting the following error: `The request was aborted: Could not create SSL/TLS secure channel`. Make sure to include the following code in the `Application_Start` or `Startup`:

```csharp
// Enable TLS 1.2
ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
```
