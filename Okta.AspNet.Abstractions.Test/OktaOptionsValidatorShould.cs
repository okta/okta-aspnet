using System;
using Xunit;
using FluentAssertions;
namespace Okta.AspNet.Abstractions.Test
{
    public class OktaOptionsValidatorShould
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailWhenClientIdIsNullOrEmpty(String clientId)
        {
            var options = new OktaOptions()
            {
                OrgUrl = OktaOptionsValidatorHelper.VALID_ORG_URL,
                ClientId = clientId,
            };

            Action action = () => new MockOktaOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == nameof(OktaOptions.ClientId));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailIfOrgUrlIsNullOrEmpty(String orgUrl)
        {
            var options = new OktaOptions()
            {
                OrgUrl = orgUrl,
                ClientId = "ClientId"
            };

            Action action = () => new MockOktaOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == nameof(OktaOptions.OrgUrl)); 
        }

        [Theory]
        [InlineData("http://myOktaDomain.oktapreview.com")]
        [InlineData("httsp://myOktaDomain.oktapreview.com")]
        [InlineData("invalidOrgUrl")]
        public void FailIfOrgUrlIsNotStartingWithHttps(String orgUrl)
        {
            var options = new OktaMvcOptions()
            {
                OrgUrl = orgUrl,
                ClientId = "ClientId"
            };

            Action action = () => new MockOktaOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == nameof(OktaOptions.OrgUrl));
        }

        [Theory]
        [InlineData("https://{Youroktadomain}.com")]
        [InlineData("https://{yourOktaDomain}.com")]
        [InlineData("https://{YourOktaDomain}.com")]
        public void FailIfOrgUrlIsNotDefined(String orgUrl)
        {
            var options = new OktaMvcOptions()
            {
                OrgUrl = orgUrl,
                ClientId = "ClientId"
            };

            Action action = () => new MockOktaOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == nameof(OktaOptions.OrgUrl));
        }

        [Fact]
        public void FailIfOrgUrlIsIncludingAdmin()
        {
            var options = new OktaMvcOptions()
            {
                OrgUrl = "https://myOktaOrg-admin.oktapreview.com",
                ClientId = "ClientId"
            };

            Action action = () => new MockOktaOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == nameof(OktaOptions.OrgUrl));
        }

        [Fact]
        public void FailIfOrgUrlHasTypo()
        {
            var options = new OktaMvcOptions()
            {
                OrgUrl = "https://myOktaDomain.oktapreview.com.com",
                ClientId = "ClientId"
            };

            Action action = () => new MockOktaOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == nameof(OktaOptions.OrgUrl));
        }
    }
}

