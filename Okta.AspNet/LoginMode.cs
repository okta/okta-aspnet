// <copyright file="LoginMode.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

namespace Okta.AspNet
{
    /// <summary>
    /// LoginMode controls the login redirect behavior of the middleware.
    /// </summary>
    public enum LoginMode
    {
        /// <summary>
        /// Indicates that the login page will be provided and hosted by Okta.
        /// </summary>
        OktaHosted,

        /// <summary>
        /// Indicates that a self-hosted login page will be provider by the user.
        /// </summary>
        SelfHosted,
    }
}
