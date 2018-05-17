// <copyright file="MessageController.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;

namespace Okta.AspNet.WebApi.IntegrationTest.Controllers
{
    [Authorize]
    public class MessageController : ApiController
    {
        [HttpGet]
        [Route("~/api/messages")]
        public IEnumerable<string> Get()
        {
            var principal = RequestContext.Principal.Identity as ClaimsIdentity;

            var login = principal.Claims
                .SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                ?.Value;

            return new string[]
            {
                $"For {login ?? "your"} eyes only",
                "Your mission, should you choose to accept it...",
            };
        }
    }
}
