using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Okta.AspNetCore.Mvc.IntegrationTest
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
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            Func<RedirectContext, Task> myRedirectEvent = context =>
            {
                context.ProtocolMessage.SetParameter("myCustomParamKey", "myCustomParamValue");
                return Task.CompletedTask;
            };

            var events = new OpenIdConnectEvents
            {
                OnRedirectToIdentityProvider = myRedirectEvent,
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CustomScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOktaMvc(CustomScheme, new OktaMvcOptions()
            {
                ClientId = Configuration["Okta:ClientId"],
                ClientSecret = Configuration["Okta:ClientSecret"],
                OktaDomain = Configuration["Okta:OktaDomain"],
                OpenIdConnectEvents = events,
                
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
                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
