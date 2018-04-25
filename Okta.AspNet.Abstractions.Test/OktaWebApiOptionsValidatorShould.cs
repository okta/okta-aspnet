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
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailWhenClientIdIsNullOrEmpty(String clientId)
        {
            var options = new OktaWebApiOptions()
            {
                OrgUrl = "OrgUrl",
                ClientId = clientId,
            };

            ShouldFailValidation(options, nameof(OktaWebApiOptions.ClientId));
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

            ShouldFailValidation(options, nameof(OktaWebApiOptions.OrgUrl));
        }

        private void ShouldFailValidation(OktaWebApiOptions options, string paramName)
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
    }
}

