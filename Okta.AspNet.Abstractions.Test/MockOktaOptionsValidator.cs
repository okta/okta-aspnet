// <copyright file="MockOktaOptionsValidator.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

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
