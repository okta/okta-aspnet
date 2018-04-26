using Okta.AspNet.Abstractions;
using System;
using Xunit;

namespace Okta.AspNet.Abstractions.Test
{
    public class OktaMvcOptionsValidatorShould
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailWhenClientSecretIsNullOrEmpty(String clientSecret)
        {
            var options = new OktaMvcOptions()
            {
                OrgUrl = "OrgUrl",
                ClientId = "ClientId",
                ClientSecret = clientSecret
            };

            ShouldFailValidation(options, nameof(OktaMvcOptions.ClientSecret));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailWhenRedirectUriIsNullOrEmpty(String redirectUri)
        {
            var options = new OktaMvcOptions()
                {
                    OrgUrl = "OrgUrl",
                    ClientId = "ClientId",
                    ClientSecret = "ClientSecret",
                    RedirectUri = redirectUri
                };

            ShouldFailValidation(options, nameof(OktaMvcOptions.RedirectUri));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailWhenClientIdIsNullOrEmpty(String clientId)
        {
            var options = new OktaMvcOptions()
            {
                OrgUrl = "OrgUrl",
                ClientId = clientId,
            };

            ShouldFailValidation(options, nameof(OktaMvcOptions.ClientId));
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

            ShouldFailValidation(options, nameof(OktaMvcOptions.OrgUrl));
        }

        [Fact]
        public void NotThrowWhenParamsAreProvided()
        {
            var options = new OktaMvcOptions()
            {
                OrgUrl = "OrgUrl",
                ClientId = "ClientId",
                ClientSecret = "ClientSecret",
                RedirectUri = "RedirectUri"
            };

            new OktaMvcOptionsValidator().Validate(options);
            Assert.True(true, "No exception was thrown.");
        }

        private void ShouldFailValidation(OktaMvcOptions options, string paramName)
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
    }
}