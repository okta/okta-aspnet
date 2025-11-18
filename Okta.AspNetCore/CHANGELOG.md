# Changelog
Running changelog of releases since `3.2.0`

## v4.6.8

### Bug Fixes

- Fix JWT claim type mapping for .NET 8+ by enabling MapInboundClaims (#286)
  - Resolved issue where JWT claims like 'sub' and 'email' were not being mapped to standard .NET ClaimTypes (e.g., ClaimTypes.NameIdentifier, ClaimTypes.Email)
  - This regression was introduced in v4.6.5 when upgrading to IdentityModel 8.2.0, which changed the default behavior of JsonWebTokenHandler.MapInboundClaims from true to false
  - Applications using ClaimTypes constants to access user claims now work correctly on .NET 8+

## v4.6.7

### Bug Fixes

- Fix strong name signing build process validation issue (#291)

## v4.6.6
Upgraded .NET Framework from 4.6.2 to 4.8

## v4.6.5
Updated IdentityModel Package to 8.2.0

## v4.6.4
- feat: Use JsonWebTokenHandler in .NET 8 for improved token handling (#281)
- feat: Add strong name signing to Okta.AspNetCore assembly (#280)

## v4.6.3
- Vulnerable Package Upgrades
- Upgrade in Okta.AspNet.Abstractions

## v4.6.2

- Replace RuntimeInformation dependency with Environment.Version for .Net8+ (#275) for Okta.AspNet.Abstractions.

## v4.6.1

- Dependency Upgrades: Upgraded vulnerable dependency, System.IdentityModel.Tokens.Jwt to version 6.35
- Security Enhancements: Added oidcOptions.UseSecurityTokenValidator = true for .NET 8.
- Framework Compatibility: Added .NET 8 as a valid frameworkTarget for Okta.AspNet.Abstractions.
- Bug Fixes: Fixed Blazor .NET 8 Error (#260), Okta.AspNetCore (#259), (#261).

## v4.6.0

### Features

- Add support for .NET 8

## v4.5.0

### Features

- Add support for .NET 7

## v4.4.2

### Bug Fixes

- Fix "Method not found:Boolean Okta.AspNet.Abstractions.OktaParams.IsPromptEnrollAuthenticator" issue (#228)

## v4.4.1

### Features

- Update authorization request's params accordingly when `prompt=enroll_authenticator`

## v4.4.0

### Features

- Add support for `prompt` and `enroll_amr_values`

## v4.3.0

### Features

- Add support for `acr_values`

## v4.2.1

### Updates

- Add explicit reference to Newtonsoft.Json v 13.0.1
- Update Microsoft.AspNetCore.Authentication.JwtBearer to v3.1.28
- Update Microsoft.AspNetCore.Authentication.OpenIdConnect for net6.0 to v 6.23.0

## v4.2.0

### Features

- Add support for authentication scheme configuration (#191)

## v4.1.0

### Features

- Add support for .NET 6.0 framework
- Update `Microsoft.AspNetCore.Authentication.JwtBearer` and `Microsoft.AspNetCore.Authentication.OpenIdConnect` dependencies to the latest version of each target framework supported by the SDK. (#199)

## v4.0.0

### Features

- Add support for OIDC events configuration in MVC projects.
- Add support for JWT events configuration in Web API projects.
- Add support for BackchannelHttpHandler configuration.
- Add support for BackchannelTimeout configuration.

### Breaking changes

- Remove `OnTokenValidated`, `OnUserInformationReceived`, `OnOktaApiFailure` and `OnAuthenticationFailed` events in favor of `OpenIdConnectEvents` (MVC).
- Remove `ClientId` property from `WebApiOptions`.

## v3.5.1

### Features

- Add support for ASP.NET Core Identity

## v3.5.0

### Features

- Add support for .NET 5.0 framework

## v3.4.0

### Features

- Add support for `login_hint` parameter (#142)

## v3.3.0

### Features

- Add support for Proxy configuration (#123)

## v3.2.0

### Features

- Expose `OnOktaApiFailure` and `AuthenticationFailed` events.
