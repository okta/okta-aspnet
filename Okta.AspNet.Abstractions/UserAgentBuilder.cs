// <copyright file="UserAgentBuilder.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Reflection;
using System.Runtime.InteropServices;

namespace Okta.AspNet.Abstractions
{
    public class UserAgentBuilder
    {
        private const string VersionSeparator = "/";

        public static string GetUserAgent()
        {
            return string.Join(" ", GetOSVersion(), GetFrameworkVersion());
        }

        private static string GetFrameworkVersion()
        {
            var frameworkVersion = typeof(UserAgentBuilder).GetTypeInfo()
               .Assembly
               .GetName()
               .Version;

            var frameworkToken = $"okta-aspnet{VersionSeparator}{frameworkVersion.Major}.{frameworkVersion.Minor}.{frameworkVersion.Build}";

            return frameworkToken;
        }

        private static string GetOSVersion()
        {
            return $"os-version{VersionSeparator}{RuntimeInformation.OSDescription.ToString()}{VersionSeparator}{RuntimeInformation.OSArchitecture.ToString()}";
        }
    }
}
