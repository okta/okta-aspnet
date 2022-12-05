# Changelog
Running changelog of releases since `1.6.0`

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
