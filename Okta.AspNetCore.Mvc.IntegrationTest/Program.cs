using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Okta.AspNetCore.Mvc.IntegrationTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        // TODO: Create okta app or Office 365 app registration
        // TODO: Edit Constants line in csproj file to be either Okta, or AzureAD
        // TODO: Jabil developers can contact Scott Weeden for assistance
        // <PropertyGroup>
        //    <DefineConstants>$(DefineConstants);Okta</DefineConstants>
        //  </PropertyGroup> 
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
