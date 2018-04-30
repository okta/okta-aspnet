using System;
using Xunit;
using FluentAssertions;

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
                OrgUrl = OktaOptionsValidatorHelper.VALID_ORG_URL,
                ClientId = "ClientId",
                ClientSecret = clientSecret
            };

            Action action = () => new OktaMvcOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == nameof(OktaMvcOptions.ClientSecret));
        }

        [Fact]
        public void FailWhenClientSecretIsNotDefined()
        {
            var options = new OktaMvcOptions()
            {
                OrgUrl = OktaOptionsValidatorHelper.VALID_ORG_URL,
                ClientId = "ClientId",
                ClientSecret = "{ClientSecret}"
            };

            Action action = () => new OktaMvcOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentException>().Where(e => e.ParamName == nameof(OktaMvcOptions.ClientSecret));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailWhenRedirectUriIsNullOrEmpty(String redirectUri)
        {
            var options = new OktaMvcOptions()
                {
                    OrgUrl = OktaOptionsValidatorHelper.VALID_ORG_URL,
                    ClientId = "ClientId",
                    ClientSecret = "ClientSecret",
                    RedirectUri = redirectUri
                };

            Action action = () => new OktaMvcOptionsValidator().Validate(options);
            action.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == nameof(OktaMvcOptions.RedirectUri));
        }

        [Fact]
        public void NotThrowWhenParamsAreProvided()
        {
            var options = new OktaMvcOptions()
            {
                OrgUrl = OktaOptionsValidatorHelper.VALID_ORG_URL,
                ClientId = "ClientId",
                ClientSecret = "ClientSecret",
                RedirectUri = "RedirectUri"
            };

            Action action = () => new OktaMvcOptionsValidator().Validate(options);
            action.Should().NotThrow();
        }
    }
}