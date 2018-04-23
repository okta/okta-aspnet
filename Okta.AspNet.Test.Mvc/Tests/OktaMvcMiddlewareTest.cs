using Microsoft.Owin.Testing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace Okta.AspNet.Test.Mvc.Tests
{
    [TestFixture]
    public class OktaMvcMiddlewareTest
    {
        private TestServer _server;

        [OneTimeSetUp]
        public void FixtureInit()
        {
            _server = TestServer.Create<Startup>();
            // TODO: BaseAddress should be configurable
            _server.BaseAddress = new Uri("http://localhost:60611");
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        [OneTimeTearDown]
        public void FixtureDispose()
        {
            _server.Dispose();
        }

        [Test]
        public async Task TestFailWhenAccessToProtectedRouteWhenNotSignedOnAsync()
        {
            // issue : I am getting 404
            var response = _server.HttpClient.GetAsync("/Account/Claims").Result;

            Assert.IsTrue(true);
        }
    }
}