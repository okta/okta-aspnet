using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Okta.AspNet.Abstractions;

namespace Okta.AspNetCore.Mvc.IntegrationTest.Controllers
{
    public class AccountWithSchemeController : Controller
    {
        public IActionResult Login()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Challenge("CustomScheme");
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult LoginWithIdp()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                var properties = new AuthenticationProperties();
                properties.Items.Add("idp", "foo");
                properties.RedirectUri = "/Home/";

                return Challenge(properties, "CustomScheme");
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult LoginWithLoginHint()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                var properties = new AuthenticationProperties();
                properties.Items.Add(OktaParams.LoginHint, "foo");
                properties.RedirectUri = "/Home/";

                return Challenge(properties, "CustomScheme");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            return new SignOutResult(new[] { "CustomScheme", CookieAuthenticationDefaults.AuthenticationScheme });
        }

        [Authorize]
        [HttpGet]
        public IActionResult Claims()
        {
            return View(HttpContext.User.Claims);
        }
    }
}