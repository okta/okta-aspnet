// <copyright file="IUserInformationProvider.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Security.Claims;
using System.Threading.Tasks;

namespace Okta.AspNet
{
    public interface IUserInformationProvider
    {
        Task EnrichIdentityViaUserInfoAsync(ClaimsIdentity subject, string accessToken);
    }
}