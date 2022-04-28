# Okta.AspNet SDK migration guide

This library uses semantic versioning and follows Okta's [library version policy](https://developer.okta.com/code/library-versions/). In short, we don't make breaking changes unless the major version changes!

## Migrating from Okta.AspNet 2.0.0 to 2.0.1

To stay aligned with the official [.Net Framework Support Policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-framework#sha1-retirement), this library now supports .Net Framework version 4.6.2 or later and has dropped support for earlier framework versions.  Please be sure to update your deployed runtime to .NET Framework 4.6.2 or later to continue to use this library.

## Migrating from Okta.AspNet 1.x to 2.x

In previous versions, the `OktaMvcOptions` exposed the `SecurityTokenValidated` and `AuthenticationFailed` events you could hook into. Starting in 2.x series, the  `OktaMvcOptions` exposes the `OpenIdConnectEvents` property which allows you to hook into all the events provided by the uderlying OIDC middleware.

_Before:_

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

_Now:_

```csharp
   app.UseOktaMvc(new OktaMvcOptions()
        {
            // ... other configuration options removed for brevity ...
            OpenIdConnectEvents = new OpenIdConnectAuthenticationNotifications
            {
                AuthenticationFailed = OnAuthenticationFailed,
            },
        });
```
## Migrating from Okta.AspNetCore 3.x to 4.x

In previous versions, the `OktaMvcOptions` exposed the `OnTokenValidated`, `OnOktaApiFailure`, `OnUserInformationReceived` and `OnAuthenticationFailed` events you could hook into. Starting in 4.x series, the  `OktaMvcOptions` exposes the `OpenIdConnectEvents` property which allows you to hook into all the events provided by the uderlying OIDC middleware.

_Before:_

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

_Now:_

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
}
```

## Getting help

If you have questions about this library or about the Okta APIs, post a question on our [Developer Forum](https://devforum.okta.com).

If you find a bug or have a feature request for this library specifically, [post an issue](https://github.com/okta/okta-aspnet/issues) here on GitHub.
