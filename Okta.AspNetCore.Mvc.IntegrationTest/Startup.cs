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
    public class Startup
    {
        public Startup()
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
            // Use direct environment variables to match CircleCI environment setup
            var oktaDomain = Environment.GetEnvironmentVariable("OKTA_CLIENT_OKTADOMAIN") ?? Configuration["Okta:OktaDomain"];
            var clientId = Environment.GetEnvironmentVariable("OKTA_CLIENT_CLIENTID") ?? Configuration["Okta:ClientId"];
            var clientSecret = Environment.GetEnvironmentVariable("OKTA_CLIENT_CLIENTSECRET") ?? Configuration["Okta:ClientSecret"];

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
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOktaMvc(new OktaMvcOptions()
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                OktaDomain = oktaDomain,
                OpenIdConnectEvents = events,
                CallbackPath = "/signin-oidc", // Use the standard ASP.NET Core callback path
            });
            
            // Disable Pushed Authorization Requests (PAR) for tests to verify parameters in URL
            services.PostConfigure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
#if NET9_0_OR_GREATER
                options.PushedAuthorizationBehavior = Microsoft.AspNetCore.Authentication.OpenIdConnect.PushedAuthorizationBehavior.Disable;
#endif
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
