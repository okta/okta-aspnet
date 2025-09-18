# Changelog
Running changelog of releases since `1.6.0`

## v3.2.10

### Bug Fixes

- Fix strong name signing build process validation issue (#291)

## v3.2.9
Upgraded .NET Framework from 4.6.2 to 4.8

## v3.2.8
Updated IdentityModel Package to 8.2.0

## v3.2.7
- Upgrade in Okta.AspNetCore

## v3.2.6
- Vulnerable Package Upgrades
- Upgrade in Okta.AspNet.Abstractions

## v3.2.5

- Replace RuntimeInformation dependency with Environment.Version for .Net8+ (#275) for Okta.AspNet.Abstractions.

## v3.2.4

- Dependency Upgrades: Upgraded vulnerable dependency, System.IdentityModel.Tokens.Jwt to version 6.35
- Security Enhancements: Added oidcOptions.UseSecurityTokenValidator = true for .NET 8.
- Framework Compatibility: Added .NET 8 as a valid frameworkTarget for Okta.AspNet.Abstractions.
- Bug Fixes: Fixed Blazor .NET 8 Error (#260), Okta.AspNetCore (#259), (#261).

## 3.2.3

### Bug Fixes

Fix "initial request before signing keys are cached is rejected as unauthorized" issue (#243) (#249)

## 3.2.2

### Bug Fixes

- Fix "Method not found:Boolean Okta.AspNet.Abstractions.OktaParams.IsPromptEnrollAuthenticator" issue (#228)

## 3.2.1

### Features

- Update authorization request's params accordingly when `prompt=enroll_authenticator`

## v3.2.0

### Features

- Add support for `prompt` and `enroll_amr_values`

## v3.1.0

### Features

- Add support for `acr_values`


## v3.0.2

### Updates

- Update Microsoft.Owin.* dependencies to v4.2.2

## v3.0.1

### Bug Fix

- Fix handling of proxy configuration

## v3.0.0

### Updates

- Update target framework to v4.6.2
- Update IdentityModel dependency version to v6.0.0
- Add support for Authentication Type configuration (#191)

## v2.0.0

### Features

- Add support for OIDC events configuration in MVC projects.
- Add support for JWT events configuration in Web API projects.
- Add support for BackchannelHttpHandler configuration.
- Add support for BackchannelTimeout configuration.

### Breaking changes

- Remove `SecurityTokenValidated` and `AuthenticationFailed` events in favor of `OpenIdConnectEvents` (MVC).
- Remove `ClientId` property from `WebApiOptions`.

## v1.8.2

### Bug Fix

- Fix issue with strong name signature introduced in 1.8.1.

## v1.8.1 

### Features

- Add strong name signature.

## v1.8.0

### Features

- Add support for `login_hint` parameter (#142)
- Update Owin dependencies (among others) to `4.1.1`

## v1.7.0

### Features

- Add support for Proxy configuration (#123)

## v1.6.0

### Bug Fixes

- Cache signing keys and update them periodically in order to avoid deadlocks (#126)

### Features

- Expose `AuthenticationFailed` event.
