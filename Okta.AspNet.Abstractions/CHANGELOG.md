# Changelog
Running changelog of releases since `3.0.5`

## v6.0.0

### Breaking Changes

- Upgraded .NET Framework minimum requirement from 4.8 to 4.8.1
- Multi-targeted for netstandard2.0, net481, net8.0, net9.0, and net10.0

### Features

- Add support for .NET 9.0 and .NET 10.0
- Add Issuer property to OktaWebOptions for automatic URL parsing

### Improvements

- Remove unnecessary System.Runtime.InteropServices.RuntimeInformation package dependency
- Update all packages to latest versions (Microsoft.IdentityModel.* 8.15.0, System.Text.Json 10.0.0)
- Switch from DelaySign to PublicSign for strong name signing
- Fix safe code analysis warnings

## v5.1.7

### Bug Fixes

- Remove unnecessary System.Runtime.InteropServices.RuntimeInformation package dependency (#298)
- Fix strong name signing in CI builds by using full path to sn.exe (#296)

## v5.1.6

### Bug Fixes

- Fix strong name signing build process validation issue (#291)

## v5.1.5
Upgraded .NET Framework from 4.6.2 to 4.8

## v5.1.4
Updated IdentityModel Package to 8.2.0

## v5.1.3
- Upgrade in Okta.AspNetCore

## v5.1.2
- Vulnerable Package Upgrades
- Increase in Backchannel Timeout 

## v5.1.1

- Replace RuntimeInformation dependency with Environment.Version for .Net8+ (#275)

## v5.1.0

- Dependency Upgrades: Upgraded vulnerable dependency, System.IdentityModel.Tokens.Jwt to version 6.35
- Security Enhancements: Added oidcOptions.UseSecurityTokenValidator = true for .NET 8.
- Framework Compatibility: Added .NET 8 as a valid frameworkTarget for Okta.AspNet.Abstractions.
- Bug Fixes: Fixed Blazor .NET 8 Error (#260), Okta.AspNetCore (#259), (#261).

## v5.0.0

- Update target framework to v4.6.2

## v4.2.1

### Bug Fixes

- Fix "Method not found:Boolean Okta.AspNet.Abstractions.OktaParams.IsPromptEnrollAuthenticator" issue (#228)

## v4.2.0

### Features

- Add support for `prompt` and `enroll_amr_values`

## v4.1.0

### Features

- Add support for `acr_values`

## v4.0.1

### Features

- Update `Microsoft.IdentityModel.Tokens` dependency to `6.17.0`
- Update `System.IdentityModel.Tokens.Jwt` dependency to `6.17.0`

## v4.0.0

### Features

- Add support for OIDC events configuration in MVC projects.
- Add support for JWT events configuration in Web API projects.
- Add support for BackchannelHttpHandler configuration.
- Add support for BackchannelTimeout configuration.

### Breaking changes

- Remove `ClientId` property from `WebApiOptions`.

### Features

- Add strong name signature.


## v3.2.2

### Bug Fix

- Fix issue with strong name signature introduced in 3.2.1.

## v3.2.1 

### Features

- Add strong name signature.

## v3.2.0

### Features

- Update dependencies
- Add a new class called `OktaParams` that contains Okta well-known parameters supported by the SDK 
- Report .NET runtime via user agent.

## v3.1.0

### Features

- Add support for Proxy configuration (#123)

## v3.0.5

### Bug Fixes

- Update `WebApiOptions` validator to throw when the Org AS is configured. The Org AS is not supported for Web API.
