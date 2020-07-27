using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Okta.AspNet.Abstractions;
using Xunit;

namespace Okta.AspNet.Test
{
    public class OktaHttpMessageHandlerShould
    {
        [Fact]
        public async Task SetInnerHandlerWebProxy()
        {
            await Task.Run(() =>
            {
                var testProxyAddress = "http://test.cxm/";
                var testFrameworkName = $"{nameof(SetInnerHandlerWebProxy)}_testFrameworkName";
                var version = typeof(OktaHttpMessageHandlerShould).Assembly.GetName().Version;
                var oktaHandler = new OktaHttpMessageHandler(testFrameworkName, version, new OktaMvcOptions
                {
                    Proxy = new ProxyConfiguration
                    {
                        Host = testProxyAddress,
                    },
                });
                oktaHandler.InnerHandler.Should().NotBeNull();
                oktaHandler.InnerHandler.Should().BeAssignableTo<HttpClientHandler>();

                var httpClientHandler = (HttpClientHandler)oktaHandler.InnerHandler;
                httpClientHandler.Proxy.Should().NotBeNull();
                httpClientHandler.Proxy.Should().BeAssignableTo<DefaultProxy>();

                var webProxy = (DefaultProxy)httpClientHandler.Proxy;
                webProxy.GetProxy(Arg.Any<Uri>()).ToString().Should().Be(testProxyAddress);
                webProxy.Credentials.Should().BeNull();
            });
        }

        [Fact]
        public async Task SetInnerHandlerWebProxyCredentials()
        {
            await Task.Run(() =>
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
            });
        }

        [Fact]
        public async Task NotSetInnerHandlerWebProxyIfNotSpecified()
        {
            await Task.Run(() =>
            {
                var testFrameworkName = $"{nameof(NotSetInnerHandlerWebProxyIfNotSpecified)}_testFrameworkName";
                var version = typeof(OktaHttpMessageHandlerShould).Assembly.GetName().Version;
                var oktaHandler = new OktaHttpMessageHandler(testFrameworkName, version, new OktaMvcOptions());
                oktaHandler.InnerHandler.Should().NotBeNull();
                oktaHandler.InnerHandler.Should().BeAssignableTo<HttpClientHandler>();

                var httpClientHandler = (HttpClientHandler) oktaHandler.InnerHandler;
                httpClientHandler.Proxy.Should().BeNull();
            });
        }
    }
}
