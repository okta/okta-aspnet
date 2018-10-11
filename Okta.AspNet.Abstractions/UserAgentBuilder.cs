// <copyright file="UserAgentBuilder.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace Okta.AspNet.Abstractions
{
    public class UserAgentBuilder
    {
        private const string VersionSeparator = "/";
        private string _frameworkName;
        private Version _frameworkVersion;

        public UserAgentBuilder(string frameworkName, Version frameworkVersion)
        {
            _frameworkName = frameworkName;
            _frameworkVersion = frameworkVersion;
        }

        public string GetUserAgent()
        {
            return string.Join(" ", GetOSVersion(), GetFrameworkVersion());
        }

        private string GetFrameworkVersion()
        {
            return $"{_frameworkName}{VersionSeparator}{_frameworkVersion.Major}.{_frameworkVersion.Minor}.{_frameworkVersion.Build}";
        }

        private string GetOSVersion()
        {
            return $"os-version{VersionSeparator}{Environment.OSVersion.VersionString}{VersionSeparator}{Environment.OSVersion.Platform}";
        }
    }
}
