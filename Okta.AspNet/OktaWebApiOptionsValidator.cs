using Okta.AspNet.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Okta.AspNet
{
    public class OktaWebApiOptionsValidator : OktaOptionsValidator
    {
        public void Validate(OktaWebApiOptions options)
        {
            base.Validate(options);
        }
    }
}
