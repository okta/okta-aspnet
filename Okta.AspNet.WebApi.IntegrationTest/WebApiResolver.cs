using Okta.AspNet.Test.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http.Dispatcher;

namespace Okta.AspNet.Test.WebApi.Tests
{
    public class WebApiResolver : DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            return new List<Assembly> { typeof(MessageController).Assembly };
        }
    }
}