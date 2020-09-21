using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Okta.AspNet.Abstractions;
using Okta.AspNetCore;
using Xunit;

namespace Okta.AspNet.Test
{
    public class OktaHttpMessageHandlerShould
    {
        [Fact]
        public void SetInnerHandlerWebProxy()
        {
            var testProxyAddress = "http://test.cxm/";
            var testUserName = "testUserName";
            var testPassword = "testPassword";
            var testFrameworkName = $"{nameof(SetInnerHandlerWebProxy)}_testFrameworkName";
            var version = typeof(OktaHttpMessageHandlerShould).Assembly.GetName().Version;
            var oktaHandler = new OktaHttpMessageHandler(testFrameworkName, version, new OktaMvcOptions
            {
                Proxy = new ProxyConfiguration
                {
                    Host = testProxyAddress,
                    Username = testUserName,
                    Password = testPassword,
                },
            });
            oktaHandler.InnerHandler.Should().NotBeNull();
            oktaHandler.InnerHandler.Should().BeAssignableTo<HttpClientHandler>();

            var httpClientHandler = (HttpClientHandler)oktaHandler.InnerHandler; 
            httpClientHandler.Proxy.Should().NotBeNull();
            httpClientHandler.Proxy.Should().BeAssignableTo<DefaultProxy>();

            var webProxy = (DefaultProxy)httpClientHandler.Proxy;
            webProxy.GetProxy(Arg.Any<Uri>()).ToString().Should().Be(testProxyAddress);
            webProxy.Credentials.Should().NotBeNull();
            webProxy.Credentials.GetCredential(Arg.Any<Uri>(), string.Empty).UserName.Should().Be(testUserName);
            webProxy.Credentials.GetCredential(Arg.Any<Uri>(), string.Empty).Password.Should().Be(testPassword);
        }
        
        [Fact]
        public void SetInnerHandlerWebProxyPort()
        {
            var testProxyBaseUri = "http://test.cxm/";
            var testPort = 8080;
            var expectedProxyAddress = "http://test.cxm:8080/";
            var testFrameworkName = $"{nameof(SetInnerHandlerWebProxyPort)}_testFrameworkName";
            var version = typeof(OktaHttpMessageHandlerShould).Assembly.GetName().Version;
            var oktaHandler = new OktaHttpMessageHandler(testFrameworkName, version, new OktaMvcOptions
            {
                Proxy = new ProxyConfiguration
                {
                    Host = testProxyBaseUri,
                    Port = testPort,
                },
            });
            oktaHandler.InnerHandler.Should().NotBeNull();
            oktaHandler.InnerHandler.Should().BeAssignableTo<HttpClientHandler>();

            var httpClientHandler = (HttpClientHandler)oktaHandler.InnerHandler;
            httpClientHandler.Proxy.Should().NotBeNull();
            httpClientHandler.Proxy.Should().BeAssignableTo<DefaultProxy>();

            var webProxy = (DefaultProxy)httpClientHandler.Proxy;
            var proxyUri = webProxy.GetProxy(Arg.Any<Uri>());
            proxyUri.ToString().Should().Be(expectedProxyAddress);
            proxyUri.Port.Should().Be(testPort);
            webProxy.Credentials.Should().BeNull();
        }
        
        [Fact]
        public void NotSetInnerHandlerWebProxyIfNotSpecified()
        {
            var testFrameworkName = $"{nameof(NotSetInnerHandlerWebProxyIfNotSpecified)}_testFrameworkName";
            var version = typeof(OktaHttpMessageHandlerShould).Assembly.GetName().Version;
            var oktaHandler = new OktaHttpMessageHandler(testFrameworkName, version, new OktaMvcOptions());
            oktaHandler.InnerHandler.Should().NotBeNull();
            oktaHandler.InnerHandler.Should().BeAssignableTo<HttpClientHandler>();

            var httpClientHandler = (HttpClientHandler)oktaHandler.InnerHandler; 
            httpClientHandler.Proxy.Should().BeNull();
        }
    }
}
