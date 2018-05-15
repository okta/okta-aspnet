using Okta.AspNet.WebApi.IntegrationTest.Controllers;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http.Dispatcher;

namespace Okta.AspNet.WebApi.IntegrationTest
{
    public class WebApiResolver : DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            return new List<Assembly> { typeof(MessageController).Assembly };
        }
    }
}