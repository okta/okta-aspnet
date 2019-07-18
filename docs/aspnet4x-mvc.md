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

> Note: If you are using role-based authorization and you need to redirect not-authorized users to an access-denied page or similar, check out [CookieAuthenticationProvider.ApplyRedirect](https://docs.microsoft.com/en-us/previous-versions/aspnet/mt152260(v%3Dvs.113)).

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
| GetClaimsFromUserInfoEndpoint | No       | This property has been deprecated and will be no longer supported. |
| ClockSkew                 | No           | The clock skew allowed when validating tokens. The default value is 2 minutes. |
| SecurityTokenValidated                 | No           | The event invoked after the security token has passed validation and a `ClaimsIdentity` has been generated. |

You can store these values (except the Token event) in the `Web.config`, but be careful when checking in the client secret to the source control.

# Troubleshooting

If you are using .NET framework <4.6 or you are getting the following error: `The request was aborted: Could not create SSL/TLS secure channel`. Make sure to include the following code in the `Application_Start` or `Startup`:

```csharp
// Enable TLS 1.2
ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
```