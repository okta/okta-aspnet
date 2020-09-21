// <copyright file="OktaParams.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Okta.AspNet.Abstractions
{
    /// <summary>
    /// Well-known parameters used by Okta authentication.
    /// </summary>
    public static class OktaParams
    {
        /// <summary>
        /// Used to pass an Okta one-time session token. Session tokens can be obtained via the Authentication API.
        /// </summary>
        public const string SessionToken = "sessionToken";

        /// <summary>
        /// Used to prepopulate a username if prompting for authentication.
        /// </summary>
        public const string LoginHint = "login_hint";

        /// <summary>
        /// Used to provide an identity provider if there is no Okta Session.
        /// </summary>
        public const string Idp = "idp";

        /// <summary>
        /// A list with all Okta well-known params.
        /// </summary>
        public static readonly IList<string> AllParams = new List<string>() { SessionToken, Idp, LoginHint };
    }
}
