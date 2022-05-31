# Changelog
Running changelog of releases since `3.0.5`

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
