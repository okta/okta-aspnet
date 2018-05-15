using Microsoft.Extensions.Configuration;

namespace Okta.AspNetCore.Mvc.IntegrationTest.Models
{
    public class TestConfiguration
    {
        private  static IConfiguration _configuration;

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
