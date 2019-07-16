using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security;

namespace OktaAspMvcNetTest.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}