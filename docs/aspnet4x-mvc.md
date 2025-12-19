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

## Simplified Configuration (Recommended)

Starting in version 5.0.0, you can use the simplified `IConfiguration` binding to automatically load all Okta settings from your configuration:

```csharp
public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json");
        Configuration = builder.Build();
    }

    public void Configuration(IAppBuilder app)
    {
        app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
        app.UseCookieAuthentication(new CookieAuthenticationOptions());

        app.UseOktaMvc(Configuration);  // All options bound automatically from "Okta" section
    }
}
```

Add an `appsettings.json` file to your project:

```json
{
  "Okta": {
    "OktaDomain": "https://dev-123456.okta.com",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "AuthorizationServerId": "default",
    "RedirectUri": "http://localhost:8080/authorization-code/callback",
    "PostLogoutRedirectUri": "http://localhost:8080/Home",
    "Scope": "openid profile email"
  }
}
```

You can also specify a custom configuration section name:

```csharp
app.UseOktaMvc(Configuration, "MyOktaSettings");
```

> Note: The `Scope` property in configuration should be a space-separated string (e.g., `"openid profile email"`), which will be parsed into a list automatically.

## Basic configuration

If you prefer explicit control, you can still use the traditional manual configuration approach:

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
            AuthorizationServerId = "default", // Use string.Empty if you are using the Org Authorization Server
            RedirectUri = "http://localhost:8080/authorization-code/callback",
            PostLogoutRedirectUri = "http://localhost:8080/Home"
        });
    }
}
```
> Note: Starting in v3.0.0 you can now configure the authentication type: `.UseOktaMvc("myScheme", oktaMvcOptions);`.

### That's it!

Placing the `[Authorize]` attribute on your controllers or actions will check whether the user is logged in, and redirect them to Okta if necessary.

ASP.NET automatically populates `HttpContext.User` with the information Okta sends back about the user. You can check whether the user is logged in with `User.Identity.IsAuthenticated` in your actions or views.

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

> Note: The proxy configuration is ignored when an `BackchannelHttpClientHandler` is provided.

## Configure your own HttpMessageHandler implementation

Starting in Okta.AspNet 2.0.0/Okta.AspNetCore 4.0.0, you can now provide your own HttpMessageHandler implementation to be used by the uderlying OIDC middleware. This is useful if you want to log all the requests and responses to diagnose problems, or retry failed requests among other use cases. The following example shows how to provide your own logging logic via Http handlers:

```csharp

public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        app.UseOktaMvc(new OktaMvcOptions
        {
            BackchannelHttpClientHandler = new MyLoggingHandler((logger),
        });
    }
}

public class MyLoggingHandler : DelegatingHandler
{
    private readonly ILogger _logger;

    public MyLoggingHandler(ILogger logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        _logger.Trace($"Request: {request}");

        try
        {
            var response = await base.SendAsync(request, cancellationToken);
            _logger.Trace($"Response: {response}");
           
            return response;
        }
        catch (Exception ex)
        {
            _logger.Error($"Something went wrong: {ex}");
            throw;
        }
    }
}

```

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
            AuthorizationServerId = "default", // Use string.Empty if you are using the Org Authorization Server
            RedirectUri = "http://localhost:8080/authorization-code/callback",
            PostLogoutRedirectUri = "http://localhost:8080/Home",
            LoginMode = LoginMode.SelfHosted
        });
    }
}
```

> Note: If you are using role-based authorization and you need to redirect unauthorized users to an access-denied page or similar, check out [CookieAuthenticationProvider.ApplyRedirect](https://docs.microsoft.com/en-us/previous-versions/aspnet/mt152260(v%3Dvs.113)).

## Specifying the `login_hint` parameter

The `login_hint` parameter allows you to pass a username to prepopulate when prompting for authentication. For more details check out the [Okta documentation](https://developer.okta.com/docs/reference/api/oidc/#request-parameters).

Add the following action in your controller: 

```csharp
public ActionResult Login()
{
    if (!HttpContext.User.Identity.IsAuthenticated)
    {
        var properties = new AuthenticationProperties();
        properties.Dictionary.Add(OktaParams.LoginHint, "darth.vader@imperial-senate.gov");
        properties.RedirectUri = "/Home/About";

        HttpContext.GetOwinContext().Authentication.Challenge(properties,
            OktaDefaults.MvcAuthenticationType);

        return new HttpUnauthorizedResult();
    }

    return RedirectToAction("Index", "Home");
}
```

## Specifying the `acr_values`, `enroll_amr_values` and `prompt` parameters

| Parameter | Description | Required? |
| ------- |  ------------------------- | ------- |
|`acr_values` | When included in the authentication request, increases the level of user assurance. | No |
|`prompt` | Indicate the pipeline the intent of the request, such as, support enrollment of a new factor. | No |
|`enroll_amr_values` |  A space-delimited, case-sensitive string that represents a list of authenticator method references. | No |

> Note: When `prompt` is equals to `enroll_authenticator` you have to indicate the URL that Okta should send callback to after the user app sends the enrollment request. The `redirect_uri` cannot be the same as the normal OIDC flow `/authorization_code/callback` since there's no code involved in this flow. Instead, you have to specify a callback URI (which has to be added to your application's allowed URLs in your Okta dashboard) in your application where, for example, you can process the response and redirect accordingly. Also, it's expected you already have a session before triggering the authenticator enrollment flow.


For more details see the [Okta documentation](https://developer.okta.com/docs/reference/api/oidc/#request-parameters).

Add the following action in your controller: 

```csharp
public ActionResult TriggerEnroll()
{
    if (!HttpContext.User.Identity.IsAuthenticated)
    {
        var properties = new AuthenticationProperties();
        properties.Dictionary.Add(OktaParams.AcrValues, "urn:okta:loa:1fa:pwd");
        properties.Dictionary.Add(OktaParams.Prompt, "enroll_authenticator");
        properties.Dictionary.Add(OktaParams.EnrollAmrValues, "sms okta_verify");
        properties.RedirectUri = "https://localhost:44314/Account/EnrollCallback";

        HttpContext.GetOwinContext().Authentication.Challenge(properties,
            OktaDefaults.MvcAuthenticationType);

        return new HttpUnauthorizedResult();
    }

    return RedirectToAction("Index", "Home");
}

public ActionResult EnrollCallback()
{
    //...
    // If enrollment was successful
    return RedirectToAction("Index", "Home");
}
```

## Login with an external identity provider

Add the following action in your controller: 

```csharp
public ActionResult LoginWithIdp(string idp)
{
    if (!HttpContext.User.Identity.IsAuthenticated)
    {
        var properties = new AuthenticationProperties();
        properties.Dictionary.Add(OktaParams.Idp, idp);
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

## Hooking into OIDC events

This library exposes [OpenIdConnectEvents](https://docs.microsoft.com/en-us/previous-versions/aspnet/mt180963(v=vs.113)) so you can hook into specific events during the authentication process.

### Adding custom claims  

The following is an example of how to use events to add custom claims to the token:

```csharp
public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        app.UseOktaMvc(new OktaMvcOptions()
        {
            // ... other configuration options removed for brevity ...
             OpenIdConnectEvents = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = (notification) =>
                    {
                        notification.AuthenticationTicket.Identity.AddClaim(new Claim("CodeCustomClaimKey", "CodeCustomClaimValue"));

                        return Task.CompletedTask;
                    }
                },
        });
    }
}
```

> Note: For more information see [`SecurityTokenValidated`](https://learn.microsoft.com/en-us/previous-versions/aspnet/mt180993(v=vs.113))

### Handling failures

 The following is an example of how to use events to handle failures:

```csharp
public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        app.UseOktaMvc(new OktaMvcOptions()
        {
            // ... other configuration options removed for brevity ...
            OpenIdConnectEvents = new OpenIdConnectAuthenticationNotifications
            {
                AuthenticationFailed = OnAuthenticationFailed,
            },
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
> Note: For more information see [`AuthenticationFailed`](https://docs.microsoft.com/en-us/previous-versions/aspnet/mt180967(v=vs.113))

# Configuration Reference

The `OktaMvcOptions` class configures the Okta middleware. You can see all the available options in the table below:


| Property                  | Required?    | Details                         |
|---------------------------|--------------|---------------------------------|
| OktaDomain                    | **Yes**      | Your Okta domain, i.e https://dev-123456.oktapreview.com  | 
| ClientId                  | **Yes**      | The client ID of your Okta Application |
| ClientSecret              | **Yes**      | The client secret of your Okta Application |
| RedirectUri               | **Yes**      | The location Okta should redirect to process a login. This is typically `http://{yourApp}/authorization-code/callback`. No matter the value, the redirect is handled automatically by this package, so you don't need to write any custom code to handle this route. |
| AuthorizationServerId     | No           | The Okta Authorization Server to use. The default value is `default`. Use `string.Empty` if you are using the Org Authorization Server |
| PostLogoutRedirectUri     | No           | The location Okta should redirect to after logout. If blank, Okta will redirect to the Okta login page. |
| Scope                     | No           | The OAuth 2.0/OpenID Connect scopes to request when logging in. The default value is `openid profile`. |
| LoginMode                     | No           | LoginMode controls the login redirect behavior of the middleware. The default value is `OktaHosted`. |
| GetClaimsFromUserInfoEndpoint | No       | Whether to retrieve additional claims from the UserInfo endpoint after login. The default value is `true`. |
| ClockSkew                 | No           | The clock skew allowed when validating tokens. The default value is 2 minutes. |
|OpenIdConnectEvents | No | Specifies the [events](https://docs.microsoft.com/en-us/previous-versions/aspnet/dn800270(v=vs.113)) which the underlying OpenIdConnectHandler invokes to enable developer control over the authentication process.|
| Proxy                     | No           | An object describing proxy server configuration.  Properties are `Host`, `Port`, `Username` and `Password` |
| BackchannelTimeout                     | No           | Timeout value in milliseconds for back channel communications with Okta. The default value is 1 minute. |
| BackchannelHttpClientHandler                   | No           | The HttpMessageHandler used to communicate with Okta. |

You can store these values (except the Token event) in the `Web.config`, but be careful when checking in the client secret to the source control.

> Note: You can use the [The Org Authorization Server](https://developer.okta.com/docs/concepts/auth-servers/#org-authorization-server) for common use cases such as adding authentication to your MVC Application or checking user's profile, but the access token issued by this Authorization Server cannot be used or validated by your own applications.  Check out the [Okta documentation](https://developer.okta.com/docs/concepts/auth-servers/#org-authorization-server) to learn more.

# Troubleshooting

If you are using .NET framework <4.6 or you are getting the following error: `The request was aborted: Could not create SSL/TLS secure channel`. Make sure to include the following code in the `Application_Start` or `Startup`:

```csharp
// Enable TLS 1.2
ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
```
