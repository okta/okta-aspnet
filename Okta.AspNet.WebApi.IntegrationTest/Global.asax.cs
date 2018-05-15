using System.Web.Http;

namespace Okta.AspNet.WebApi.IntegrationTest
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
