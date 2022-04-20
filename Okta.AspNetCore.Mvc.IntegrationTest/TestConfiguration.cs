using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Okta.AspNetCore.Mvc.IntegrationTest
{
    public class TestConfiguration
    {
        private static IConfiguration _configuration;

        public static IConfiguration GetConfiguration()
        {
            if (_configuration == null)
            {
                _configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();
            }

            return _configuration;
        }
    }
}
