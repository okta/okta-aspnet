[<img src="https://devforum.okta.com/uploads/oktadev/original/1X/bf54a16b5fda189e4ad2706fb57cbb7a1e5b8deb.png" align="right" width="256px"/>](https://devforum.okta.com/)

[![Support](https://img.shields.io/badge/support-Developer%20Forum-blue.svg)](https://devforum.okta.com/)

Okta ASP.NET middleware
========================

* [Release status](#release-status)
* [Need help?](#need-help)
* [What you need](#what-you-need)
* [Getting started](#getting-started)
* [Resources](#resources)
* [Contributing](#contributing)


This package will add authentication via Okta to your ASP.NET or ASP.NET Core application. You can follow our instructions below, checkout our examples on GitHub or jump to our guides to see how to configure Okta with your ASP.NET applications. 

We also publish these other libraries for .NET:
 
* [Okta .NET Management SDK](https://github.com/okta/okta-sdk-dotnet)
* [Okta .NET Authentication SDK](https://github.com/okta/okta-auth-dotnet)

## Release status

This library uses semantic versioning and follows Okta's [library version policy](https://developer.okta.com/code/library-versions/).

:heavy_check_mark: The current stable major version series is: 1.x

|Library| Version | Status                    | Compatibility| Branch |
| ------- | ------- | ------------------------- | ----------------------- | -------------------- |
|`Okta.AspNet.Abstractions`| 3.x   | :heavy_check_mark: Stable | .NET Standard 2.0 and .NET Framework 4.5.2 or higher.| master |
|`Okta.AspNet`| 1.x | :heavy_check_mark: Stable | .NET Framework 4.5.2 | master |
|`Okta.AspNetCore`| 1.x | :heavy_check_mark: Stable | .NET Standard 2.0 and .NET Core 2.x | [okta-aspnetcore-2.x](https://github.com/okta/okta-aspnet/blob/okta-aspnetcore-2.x) |
|`Okta.AspNetCore`| 3.x | :heavy_check_mark: Stable | .NET Core 3.0.0 or higher | master |
 
> :warning: Note that we support both .NET Core versions 2.x and 3.x, and we use different branches, `okta-aspnetcore-2.x` and `master` respectively, for each version. 

The latest release can always be found on the [releases page][github-releases].

## Need help?
 
If you run into problems using the SDK, you can
 
* Ask questions on the [Okta Developer Forums][devforum] or email developers@okta.com.
* Post [issues][github-issues] here on GitHub (for code errors)

## What you need

An Okta account (sign up for a [forever-free developer account](https://developer.okta.com/signup/))

## Resources

Learn how to build applications using the Okta ASP.NET SDK with the following resources:

* [Guides](https://developer.okta.com/docs/guides/)
* [Quickstart ASP.NET 4.x](https://developer.okta.com/quickstart/#/okta-sign-in-page/dotnet/aspnet4)
* [Quickstart ASP.NET Core](https://developer.okta.com/quickstart/#/okta-sign-in-page/dotnet/aspnetcore)
* [ASP.NET 4.x samples](https://github.com/okta/samples-aspnet)
* [ASP.NET Core samples](https://github.com/okta/samples-aspnetcore)
* [Project documentation](https://github.com/okta/okta-aspnet/tree/master/docs)

## Contributing

Issues and Pull Requests are welcome! To build the project, clone and build it with Visual Studio 2017 or newer. Check out the [Contributing Guide](https://github.com/okta/okta-aspnet/tree/master/CONTRIBUTING.md).

[github-issues]: https://github.com/okta/okta-aspnet/issues
[github-releases]: https://github.com/okta/okta-aspnet/releases

