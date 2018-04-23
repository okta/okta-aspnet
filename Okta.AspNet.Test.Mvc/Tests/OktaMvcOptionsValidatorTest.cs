using NUnit.Framework;
using Okta.AspNet.Abstractions;
using System;

namespace Okta.AspNet.Test.Mvc.Tests
{
    [TestFixture]
    public class OktaMvcOptionsValidatorTest
    {
        [Test]
        public void TestFailIfOptionsIsNull()
        {
            TestFailIsNullOrEmpty(null, "options");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void TestFailIfOrgUrlIsNullOrEmpty(String orgUrl)
        {
            var options = new OktaMvcOptions() { OrgUrl = orgUrl };
            TestFailIsNullOrEmpty(options, "orgurl");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void TestFailIfClientIdIsNullOrEmpty(String clientId)
        {
            var options = new OktaMvcOptions() { OrgUrl = "OrgUrl", ClientId = clientId };
            TestFailIsNullOrEmpty(options, "clientid");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void TestFailIfClientSecretIsNullOrEmpty(String clientSecret)
        {
            var options = new OktaMvcOptions() { OrgUrl = "OrgUrl", ClientId = "ClientId" };
            TestFailIsNullOrEmpty(options, "clientsecret");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void TestFailIfRedirectUriIsNullOrEmpty(String redirectUri)
        {
            var options = new OktaMvcOptions()
                {
                    OrgUrl = "OrgUrl",
                    ClientId = "ClientId",
                    ClientSecret = "ClientSecret"
                };

            TestFailIsNullOrEmpty(options, "redirecturi");
        }

        public void TestFailIsNullOrEmpty(OktaMvcOptions options, string paramName)
        {
            try
            {
                new OktaMvcOptionsValidator().Validate(options);
                Assert.Fail("No exception was thrown.");
            }
            catch (ArgumentNullException e)
            {
                StringAssert.Contains(e.ParamName.ToLower(), paramName);
            }
        }
    }
}