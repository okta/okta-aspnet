# Okta.AspNet SDK migration guide

This library uses semantic versioning and follows Okta's [library version policy](https://developer.okta.com/code/library-versions/). In short, we don't make breaking changes unless the major version changes!

## Migrating from Okta.AspNetCore 4.x to 5.x

Version 5.0.0 removes support for end-of-life .NET frameworks and adds support for the latest .NET versions.

### Breaking Changes

- **Removed support for EOL .NET frameworks**: .NET Core 3.x, .NET 5.0, .NET 6.0, and .NET 7.0 are no longer supported
- **Supported frameworks**: .NET 8.0, .NET 9.0, and .NET 10.0 only
- **Dependency updates**: Microsoft.IdentityModel.* packages updated to 8.15.0, Microsoft.Extensions.Configuration updated to 10.0.0

### Migration Steps

1. Upgrade your application to .NET 8.0, .NET 9.0, or .NET 10.0
2. Update package reference: `<PackageReference Include="Okta.AspNetCore" Version="5.0.0" />`
3. No code changes required - all APIs remain backward compatible within .NET 8.0+

### New Features (Optional)

You can now use simplified IConfiguration binding:

```csharp
// Before (still works)
services.AddOktaMvc(new OktaMvcOptions
{
    OktaDomain = "https://your-domain.okta.com",
    ClientId = "your-client-id",
    ClientSecret = "your-client-secret",
    AuthorizationServerId = "default"
});

// After (new simplified approach)
services.AddOktaMvc(configuration); // Reads from "Okta" section in appsettings.json
```

## Migrating from Okta.AspNet 2.x to 3.x

Version 3.0.0 aligned with the official [.NET Framework Support Policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-framework) by supporting .NET Framework 4.6.2 or later.

### Breaking Changes

- **Minimum .NET Framework version**: Now requires .NET Framework 4.6.2 or later
- Dropped support for .NET Framework 4.5.2 and earlier versions

### Migration Steps

1. Update your deployed runtime to .NET Framework 4.6.2 or later
2. Update package reference: `<PackageReference Include="Okta.AspNet" Version="3.x" />`
3. No code changes required

## Migrating from Okta.AspNet 3.x to 4.x

Version 4.0.0 updates to .NET Framework 4.8.1 and adds new configuration features.

### Breaking Changes

- **Minimum .NET Framework version**: Now requires .NET Framework 4.8.1
- **Dependency updates**: Microsoft.IdentityModel.* packages updated to 8.15.0, Microsoft.Extensions.Configuration updated to 10.0.0

### Migration Steps

1. Upgrade your application to .NET Framework 4.8.1
2. Update package reference: `<PackageReference Include="Okta.AspNet" Version="4.0.0" />`
3. No code changes required - all APIs remain backward compatible

### New Features (Optional)

You can now use simplified IConfiguration binding:

```csharp
// Before (still works)
app.UseOktaMvc(new OktaMvcOptions
{
    OktaDomain = "https://your-domain.okta.com",
    ClientId = "your-client-id",
    ClientSecret = "your-client-secret",
    AuthorizationServerId = "default"
});

// After (new simplified approach)
app.UseOktaMvc(configuration); // Reads from "Okta" section in Web.config or appsettings.json
```

## Migrating from Okta.AspNet 2.0.0 to 3.0.0

To stay aligned with the official [.Net Framework Support Policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-framework), this library now supports .Net Framework version 4.6.2 or later and has dropped support for earlier framework versions.  Please be sure to update your deployed runtime to .NET Framework 4.6.2 or later to continue to use this library.

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
