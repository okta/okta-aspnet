using Okta.AspNet.Abstractions;
using System;
using Xunit;

namespace Okta.AspNet.Abstractions.Test
{
    public class OktaMvcOptionsValidatorShould
    {
        private static readonly string VALID_ORG_URL = "https://myOktaDomain.oktapreview.com";

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailWhenClientSecretIsNullOrEmpty(String clientSecret)
        {
            var options = new OktaMvcOptions()
            {
                OrgUrl = VALID_ORG_URL,
                ClientId = "ClientId",
                ClientSecret = clientSecret
            };

            ShouldFailWhenArgumentIsNullOrEmpty(options, nameof(OktaMvcOptions.ClientSecret));
        }

        [Fact]
        public void FailWhenClientSecretIsNotDefined()
        {
            var options = new OktaMvcOptions()
            {
                OrgUrl = VALID_ORG_URL,
                ClientId = "ClientId",
                ClientSecret = "{ClientSecret}"
            };

            ShouldFailWhenArgumentIsNullOrEmpty(options, nameof(OktaMvcOptions.ClientSecret));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailWhenRedirectUriIsNullOrEmpty(String redirectUri)
        {
            var options = new OktaMvcOptions()
                {
                    OrgUrl = VALID_ORG_URL,
                    ClientId = "ClientId",
                    ClientSecret = "ClientSecret",
                    RedirectUri = redirectUri
                };

            ShouldFailWhenArgumentIsNullOrEmpty(options, nameof(OktaMvcOptions.RedirectUri));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailWhenClientIdIsNullOrEmpty(String clientId)
        {
            var options = new OktaMvcOptions()
            {
                OrgUrl = VALID_ORG_URL,
                ClientId = clientId,
            };

            ShouldFailWhenArgumentIsNullOrEmpty(options, nameof(OktaMvcOptions.ClientId));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailIfOrgUrlIsNullOrEmpty(String orgUrl)
        {
            var options = new OktaMvcOptions()
            {
                OrgUrl = orgUrl,
                ClientId = "ClientId"
            };

            ShouldFailWhenArgumentIsNullOrEmpty(options, nameof(OktaMvcOptions.OrgUrl));
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

            ShouldFailWhenArgumentIsInvalid(options, nameof(OktaMvcOptions.OrgUrl));
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

            ShouldFailWhenArgumentIsInvalid(options, nameof(OktaMvcOptions.OrgUrl));
        }

        [Fact]
        public void FailIfOrgUrlIsIncludingAdmin()
        {
            var options = new OktaMvcOptions()
            {
                OrgUrl = "https://myOktaOrg-admin.oktapreview.com",
                ClientId = "ClientId"
            };

            ShouldFailWhenArgumentIsInvalid(options, nameof(OktaMvcOptions.OrgUrl));
        }

        [Fact]
        public void FailIfOrgUrlHasTypo()
        {
            var options = new OktaMvcOptions()
            {
                OrgUrl = "https://myOktaDomain.oktapreview.com.com",
                ClientId = "ClientId"
            };

            ShouldFailWhenArgumentIsInvalid(options, nameof(OktaMvcOptions.OrgUrl));
        }

        [Fact]
        public void NotThrowWhenParamsAreProvided()
        {
            var options = new OktaMvcOptions()
            {
                OrgUrl = VALID_ORG_URL,
                ClientId = "ClientId",
                ClientSecret = "ClientSecret",
                RedirectUri = "RedirectUri"
            };

            new OktaMvcOptionsValidator().Validate(options);
            Assert.True(true, "No exception was thrown.");
        }

        private void ShouldFailWhenArgumentIsNullOrEmpty(OktaMvcOptions options, string paramName)
        {
            try
            {
                new OktaMvcOptionsValidator().Validate(options);
                Assert.True(false, "No exception was thrown.");
            }
            catch (ArgumentNullException e)
            {
                Assert.Contains(e.ParamName, paramName);
            }
        }

        private void ShouldFailWhenArgumentIsInvalid(OktaMvcOptions options, string paramName)
        {
            try
            {
                new OktaMvcOptionsValidator().Validate(options);
                Assert.True(false, "No exception was thrown.");
            }
            catch (ArgumentException e)
            {
                Assert.Contains(e.ParamName, paramName);
            }
        }
    }
}