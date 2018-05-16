// <copyright file="MessageController.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Okta.AspNetCore.WebApi.IntegrationTest.Controllers
{
    [Produces("application/json")]
    [Authorize]
    public class MessageController : Controller
    {
        [HttpGet]
        [Route("~/api/messages")]
        public IEnumerable<string> Get()
        {
            var principal = HttpContext.User.Identity as ClaimsIdentity;

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
