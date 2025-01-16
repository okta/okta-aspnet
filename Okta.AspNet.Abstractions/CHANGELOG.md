# Changelog
Running changelog of releases since `3.0.5`

## v5.1.1
- Replace RuntimeInformation dependency with Environment.Version for .Net8+ (#275)

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
