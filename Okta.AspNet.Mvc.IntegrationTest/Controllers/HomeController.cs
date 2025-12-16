// <copyright file="HomeController.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System.Web.Mvc;
using Microsoft.Owin.Security;

namespace Okta.AspNet.Mvc.IntegrationTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Content("Home Page");
        }

        [Authorize]
        public ActionResult Protected()
        {
            return Content("Protected Page");
        }

        public ActionResult Login()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return new HttpUnauthorizedResult();
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Claims()
        {
            return Content($"Claims Count: {HttpContext.User.Identity.Name}");
        }
    }
}
