// <copyright file="TestConfiguration.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Configuration;

namespace Okta.AspNetCore.Mvc.IntegrationTest.Models
{
    public class TestConfiguration
    {
        private static IConfiguration _configuration;

        public static IConfiguration GetConfiguration()
        {
            if (_configuration == null)
            {
                _configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
            }

            return _configuration;
        }
    }
}
