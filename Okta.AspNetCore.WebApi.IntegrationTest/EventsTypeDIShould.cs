// <copyright file="EventsTypeDIShould.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Okta.AspNetCore.WebApi.IntegrationTest
{
    /// <summary>
    /// Integration tests for - EventsType DI support.
    /// These tests verify that custom JwtBearerEvents classes can be resolved from DI
    /// with constructor injection working correctly.
    /// </summary>
    public sealed class EventsTypeDIShould : IDisposable
    {
        private readonly TestServer _server;
        private readonly IHost _host;
        private readonly string _baseUrl;
        private readonly string _protectedEndpoint;

        public EventsTypeDIShould()
        {
            var configuration = TestConfiguration.GetConfiguration();
            _baseUrl = "http://localhost:58534";
            _protectedEndpoint = $"{_baseUrl}/api/messages";

            _host = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .UseStartup<StartupWithEventsTypeDI>()
                        .UseConfiguration(configuration);
                })
                .Build();

            _host.Start();
            _server = _host.GetTestServer();
            _server.BaseAddress = new Uri(_baseUrl);
        }

        [Fact]
        public async Task ResolveCustomEventsFromDI_WhenEventsTypeIsConfigured()
        {
            // Arrange
            var accessToken = "thisIsAnInvalidToken";
            var request = new HttpRequestMessage(HttpMethod.Get, _protectedEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Act
            using var client = new HttpClient(_server.CreateHandler());
            var response = await client.SendAsync(request);

            // Assert - The custom events class should have been invoked via DI
            // The header proves that our DI-resolved events class was used
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
            Assert.True(response.Headers.Contains("X-Events-DI-Test"), 
                "Custom header from DI-resolved JwtBearerEvents should be present");
            Assert.True(response.Headers.Contains("X-Logger-Injected"),
                "ILogger should have been injected into the custom events class");
        }

        [Fact]
        public async Task InjectServicesIntoCustomEvents_WhenUsingEventsType()
        {
            // Arrange
            var accessToken = "anotherInvalidToken";
            var request = new HttpRequestMessage(HttpMethod.Get, _protectedEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Act
            using var client = new HttpClient(_server.CreateHandler());
            var response = await client.SendAsync(request);

            // Assert - Verify that the custom service was injected and used
            Assert.True(response.Headers.Contains("X-Custom-Service-Used"),
                "Custom service should have been injected and used in events handler");
        }

        public void Dispose()
        {
            _server.Dispose();
            _host.Dispose();
        }
    }

    /// <summary>
    /// A custom service to demonstrate DI injection into JwtBearerEvents.
    /// </summary>
    public interface ICustomAuthService
    {
        string GetAuthInfo();
    }

    /// <summary>
    /// Implementation of the custom auth service.
    /// </summary>
    public class CustomAuthService : ICustomAuthService
    {
        public string GetAuthInfo() => "CustomAuthServiceWasInjected";
    }

    /// <summary>
    /// Custom JwtBearerEvents that receives services via constructor injection.
    /// </summary>
    public class CustomJwtBearerEventsWithDI : JwtBearerEvents
    {
        private readonly ILogger<CustomJwtBearerEventsWithDI> _logger;
        private readonly ICustomAuthService _customAuthService;

        public CustomJwtBearerEventsWithDI(
            ILogger<CustomJwtBearerEventsWithDI> logger,
            ICustomAuthService customAuthService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _customAuthService = customAuthService ?? throw new ArgumentNullException(nameof(customAuthService));
        }

        public override Task Challenge(JwtBearerChallengeContext context)
        {
            // Prove that this DI-resolved events class was invoked
            context.HttpContext.Response.Headers["X-Events-DI-Test"] = "true";

            // Prove that ILogger was successfully injected
            if (_logger != null)
            {
                _logger.LogInformation("CustomJwtBearerEventsWithDI.Challenge invoked");
                context.HttpContext.Response.Headers["X-Logger-Injected"] = "true";
            }

            // Prove that custom service was successfully injected
            var authInfo = _customAuthService.GetAuthInfo();
            if (!string.IsNullOrEmpty(authInfo))
            {
                context.HttpContext.Response.Headers["X-Custom-Service-Used"] = authInfo;
            }

            return base.Challenge(context);
        }

        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            _logger.LogWarning(context.Exception, "Authentication failed - logged via injected ILogger");
            return base.AuthenticationFailed(context);
        }
    }

    /// <summary>
    /// Startup class that configures Okta with EventsType for DI support.
    /// </summary>
    public class StartupWithEventsTypeDI
    {
        public StartupWithEventsTypeDI()
        {
            var builder = new ConfigurationBuilder();
            builder
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Register the custom auth service
            services.AddScoped<ICustomAuthService, CustomAuthService>();

            // Register the custom JwtBearerEvents - this is the key for DI support
            services.AddScoped<CustomJwtBearerEventsWithDI>();

            var oktaDomain = Environment.GetEnvironmentVariable("OKTA_CLIENT_OKTADOMAIN") 
                ?? Configuration["Okta:OktaDomain"];

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddOktaWebApi(new OktaWebApiOptions()
            {
                OktaDomain = oktaDomain,
                // Use EventsType instead of Events instance - THIS IS THE NEW FEATURE
                JwtBearerEventsType = typeof(CustomJwtBearerEventsWithDI),
            });

            services.AddAuthorization();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
