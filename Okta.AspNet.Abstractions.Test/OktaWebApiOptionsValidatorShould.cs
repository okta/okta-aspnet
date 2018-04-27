using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Okta.AspNet.Abstractions.Test
{
    public class OktaWebApiOptionsValidatorShould
    {
        private static readonly string VALID_ORG_URL = "https://myOktaDomain.oktapreview.com";

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailWhenClientIdIsNullOrEmpty(String clientId)
        {
            var options = new OktaWebApiOptions()
            {
                OrgUrl = VALID_ORG_URL,
                ClientId = clientId,
            };

            ShouldFailWhenArgumentIsNullOrEmpty(options, nameof(OktaWebApiOptions.ClientId));
        }

        [Theory]
        [InlineData("http://myOktaDomain.oktapreview.com")]
        [InlineData("httsp://myOktaDomain.oktapreview.com")]
        [InlineData("invalidOrgUrl")]
        public void FailIfOrgUrlIsNotStartingWithHttps(String orgUrl)
        {
            var options = new OktaWebApiOptions()
            {
                OrgUrl = orgUrl,
                ClientId = "ClientId"
            };

            ShouldFailWhenArgumentIsInvalid(options, nameof(OktaWebApiOptions.OrgUrl));
        }

        [Theory]
        [InlineData("https://{Youroktadomain}.com")]
        [InlineData("https://{yourOktaDomain}.com")]
        [InlineData("https://{YourOktaDomain}.com")]
        public void FailIfOrgUrlIsNotDefined(String orgUrl)
        {
            var options = new OktaWebApiOptions()
            {
                OrgUrl = orgUrl,
                ClientId = "ClientId"
            };

            ShouldFailWhenArgumentIsInvalid(options, nameof(OktaWebApiOptions.OrgUrl));
        }

        [Fact]
        public void FailIfOrgUrlIsIncludingAdmin()
        {
            var options = new OktaWebApiOptions()
            {
                OrgUrl = "https://myOktaDomain-admin.oktapreview.com",
                ClientId = "ClientId"
            };

            ShouldFailWhenArgumentIsInvalid(options, nameof(OktaWebApiOptions.OrgUrl));
        }

        [Fact]
        public void FailIfOrgUrlHasTypo()
        {
            var options = new OktaWebApiOptions()
            {
                OrgUrl = "https://myOktaDomain.oktapreview.com.com",
                ClientId = "ClientId"
            };

            ShouldFailWhenArgumentIsInvalid(options, nameof(OktaWebApiOptions.OrgUrl));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailIfOrgUrlIsNullOrEmpty(String orgUrl)
        {
            var options = new OktaWebApiOptions()
            {
                OrgUrl = orgUrl,
                ClientId = "ClientId"
            };

            ShouldFailWhenArgumentIsNullOrEmpty(options, nameof(OktaWebApiOptions.OrgUrl));
        }

        [Fact]
        public void NotThrowWhenParamsAreProvided()
        {
            var options = new OktaWebApiOptions()
            {
                OrgUrl = VALID_ORG_URL,
                ClientId = "ClientId",
            };

            new OktaWebApiOptionsValidator().Validate(options);
            Assert.True(true, "No exception was thrown.");
        }

        private void ShouldFailWhenArgumentIsNullOrEmpty(OktaWebApiOptions options, string paramName)
        {
            try
            {
                new OktaWebApiOptionsValidator().Validate(options);
                Assert.True(false, "No exception was thrown.");
            }
            catch (ArgumentNullException e)
            {
                Assert.Contains(e.ParamName, paramName);
            }
        }

        private void ShouldFailWhenArgumentIsInvalid(OktaWebApiOptions options, string paramName)
        {
            try
            {
                new OktaWebApiOptionsValidator().Validate(options);
                Assert.True(false, "No exception was thrown.");
            }
            catch (ArgumentException e)
            {
                Assert.Contains(e.ParamName, paramName);
            }
        }
    }
}

