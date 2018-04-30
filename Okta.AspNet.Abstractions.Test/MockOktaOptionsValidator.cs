using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.AspNet.Abstractions.Test
{
    public class MockOktaOptionsValidator : OktaOptionsValidator
    {
        protected override void ValidateOptions(OktaOptions options)
        {
            return;
        }
    }
}
