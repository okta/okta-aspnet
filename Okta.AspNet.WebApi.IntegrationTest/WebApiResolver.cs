// <copyright file="WebApiResolver.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Reflection;
using System.Web.Http.Dispatcher;
using Okta.AspNet.WebApi.IntegrationTest.Controllers;

namespace Okta.AspNet.WebApi.IntegrationTest
{
    public class WebApiResolver : DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            return new List<Assembly> { typeof(MessageController).Assembly };
        }
    }
}
