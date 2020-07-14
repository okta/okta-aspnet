// <copyright file="OktaWebApiOptionsValidator.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Okta.AspNet.Abstractions
{
    public sealed class OktaWebApiOptionsValidator : OktaWebOptionsValidator<OktaWebApiOptions>
    {
        protected override void ValidateInternal(OktaWebApiOptions options)
        {
            if (string.IsNullOrEmpty(options.AuthorizationServerId))
            {
                throw new ArgumentException("The Org Authorization Server is not supported.", nameof(options.AuthorizationServerId));
            }
        }
    }
}
