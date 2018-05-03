using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Okta.AspNet.Abstractions;
using Okta.AspNetCore.Mvc.Example.Models;

namespace Okta.AspNetCore.Mvc.Example
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var oktaConfigSettings = new OktaConfigurationSettings();
            Configuration.GetSection("Okta").Bind(oktaConfigSettings);
            
            services.AddOktaMvc(new OktaMvcOptions()
            {
                ClientId = oktaConfigSettings.ClientId,
                ClientSecret = oktaConfigSettings.ClientSecret,
                OrgUrl = oktaConfigSettings.OrgUrl
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
