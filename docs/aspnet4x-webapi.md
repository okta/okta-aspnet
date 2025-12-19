[<img src="https://devforum.okta.com/uploads/oktadev/original/1X/bf54a16b5fda189e4ad2706fb57cbb7a1e5b8deb.png" align="right" width="256px"/>](https://devforum.okta.com/)

[![Support](https://img.shields.io/badge/support-Developer%20Forum-blue.svg)](https://devforum.okta.com/)

# Installing the package

You can install the package by using the NuGet Package Explorer to search for [Okta.AspNet](https://nuget.org/packages/Okta.AspNet).

Or, you can use the `dotnet` command:

```
dotnet add package Okta.AspNet
```
# Usage example

Okta plugs into your OWIN Startup class with the `UseOktaWebApi()` method:

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
        app.UseOktaWebApi(Configuration);  // All options bound automatically from "Okta" section
    }
}
```

Add an `appsettings.json` file to your project:

```json
{
  "Okta": {
    "OktaDomain": "https://dev-123456.okta.com",
    "AuthorizationServerId": "default",
    "Audience": "api://default"
  }
}
```

You can also specify a custom configuration section name:

```csharp
app.UseOktaWebApi(Configuration, "MyOktaSettings");
```

## Manual Configuration

If you prefer explicit control, you can still use the traditional manual configuration approach:

```csharp
public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        app.UseOktaWebApi(new OktaWebApiOptions
        {
            OktaDomain = "https://{yourOktaDomain}",
            AuthorizationServerId = "default"
        });
    }
}
```
> Note: Starting in v3.0.0 you can now configure the authentication type: `.UseOktaWebApi("myScheme", oktaMvcOptions);`.

## That's it!

Placing the `[Authorize]` attribute on your controllers or actions will require a valid access token for those routes. This package will [parse and validate the access token](https://developer.okta.com/blog/2017/06/21/what-the-heck-is-oauth#oauth-flows) and populate `Http.Context` with a limited set of user information.

## Proxy configuration

If your application requires proxy server settings, specify the `Proxy` property on `OktaWebApiOptions`.

```csharp
public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        app.UseOktaWebApi(new OktaWebApiOptions
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
> Note: The proxy configuration is ignored when a `BackchannelHttpClientHandler` is provided.

## Configure your own HttpMessageHandler implementation

Starting in Okta.AspNet 2.0.0/Okta.AspNetCore 4.0.0, you can now provide your own HttpMessageHandler implementation to be used by the uderlying OIDC middleware. This is useful if you want to log all the requests and responses to diagnose problems, or retry failed requests among other use cases. The following example shows how to provide your own logging logic via Http handlers:

```csharp

public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        app.UseOktaMvc(new OktaWebApiOptions
        {
            BackchannelHttpClientHandler = new MyLoggingHandler((logger),
        });
    }
}

public class MyLoggingHandler : DelegatingHandler
{
    private readonly ILogger _logger;

    public MyLoggingHandler(ILogger logger) : base(new HttpClientHandler())
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

# Configuration Reference

The `OktaWebApiOptions` class configures the Okta middleware. You can see all the available options in the table below:

| Property                  | Required?    | Details                         |
|---------------------------|--------------|---------------------------------|
| OktaDomain                    | **Yes**      | Your Okta domain, i.e https://dev-123456.oktapreview.com  | 
| ClientId                  | No      | The client ID of your Okta Application. This property is obsolete and will be removed in the next major version. |
| AuthorizationServerId     | No           | The [Okta Custom Authorization Server](https://developer.okta.com/docs/concepts/auth-servers/#custom-authorization-server) to use. The default value is `default`. |
| Audience                  | No           | The expected audience of incoming tokens. The default value is `api://default`. |
| ClockSkew                 | No           | The clock skew allowed when validating tokens. The default value is 2 minutes. |
| Proxy                     | No           | An object describing proxy server configuration.  Properties are `Host`, `Port`, `Username` and `Password` |
 OAuthBearerAuthenticationProvider | No | The [authentication provider](https://docs.microsoft.com/en-us/previous-versions/aspnet/dn253813(v=vs.113)) which specifies callback methods invoked by the underlying authentication middleware to enable developer control over the authentication process. | 
| BackchannelTimeout                     | No           | Timeout value in milliseconds for back channel communications with Okta. The default value is 1 minute. |
| BackchannelHttpClientHandler                   | No           | The HttpMessageHandler used to communicate with Okta. |

You can store these values in the `Web.config`.

> Note: [The Org Authorization Server](https://developer.okta.com/docs/concepts/auth-servers/#org-authorization-server) is not supported for Web API because the access token issued by this Authorization Server cannot be validated by your own application. Check out the [Okta documentation](https://developer.okta.com/docs/concepts/auth-servers/#org-authorization-server) to learn more.

# Troubleshooting

If you are using .NET framework <4.6 or you are getting the following error: `The request was aborted: Could not create SSL/TLS secure channel`. Make sure to include the following code in the `Application_Start` or `Startup`:

```csharp
// Enable TLS 1.2
ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
```
