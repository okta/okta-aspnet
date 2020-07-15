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

## That's it!

Placing the `[Authorize]` attribute on your controllers or actions will require a valid access token for those routes. This package will [parse and validate the access token](https://developer.okta.com/blog/2017/06/21/what-the-heck-is-oauth#oauth-flows) and populate `Http.Context` with a limited set of user information.

# Configuration Reference

The `OktaWebApiOptions` class configures the Okta middleware. You can see all the available options in the table below:

| Property                  | Required?    | Details                         |
|---------------------------|--------------|---------------------------------|
| OktaDomain                    | **Yes**      | Your Okta domain, i.e https://dev-123456.oktapreview.com  | 
| ClientId                  | No      | The client ID of your Okta Application. This property is obsolete and will be removed in the next major version. |
| AuthorizationServerId     | No           | The [Okta Custom Authorization Server](https://developer.okta.com/docs/concepts/auth-servers/#custom-authorization-server) to use. The default value is `default`. |
| Audience                  | No           | The expected audience of incoming tokens. The default value is `api://default`. |
| ClockSkew                 | No           | The clock skew allowed when validating tokens. The default value is 2 minutes. |

You can store these values in the `Web.config`.

> Note: [The Org Authorization Server](https://developer.okta.com/docs/concepts/auth-servers/#org-authorization-server) is not supported for Web API because the access token issued by this Authorization Server cannot be validated by your own application. Check out the [Okta documentation](https://developer.okta.com/docs/concepts/auth-servers/#org-authorization-server) to learn more.

# Troubleshooting

If you are using .NET framework <4.6 or you are getting the following error: `The request was aborted: Could not create SSL/TLS secure channel`. Make sure to include the following code in the `Application_Start` or `Startup`:

```csharp
// Enable TLS 1.2
ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
```
