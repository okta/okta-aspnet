using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Okta.AspNetCore.WebApi.IntegrationTest
{
    public class StartupUsingCustomScheme
    {
        private const string CustomScheme = "CustomScheme";

        public StartupUsingCustomScheme()
        {
            var builder = new ConfigurationBuilder();
            builder
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            JwtBearerEvents events = new JwtBearerEvents();
            events.OnChallenge = context =>
            {
                context.HttpContext.Response.Headers.Add("myCustomHeader", "myCustomValue");
                return Task.CompletedTask;
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CustomScheme;
                options.DefaultChallengeScheme = CustomScheme;
                options.DefaultSignInScheme = CustomScheme;
            })
            .AddOktaWebApi(CustomScheme, new OktaWebApiOptions()
            {
                OktaDomain = Configuration["Okta:OktaDomain"],
                JwtBearerEvents = events,
            });

            services.AddAuthorization();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
