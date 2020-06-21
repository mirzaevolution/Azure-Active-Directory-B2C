using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebApp1.Controllers
{
    public class AuthController : Controller
    {

        public IActionResult Login()
        {
            string redirectUrl = Url.Action("Index", "Home", null, Request.Scheme);
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            });
        }

        [Authorize]
        public IActionResult Logout()
        {
            string redirectUrl = Url.Action("Index", "Home", null, Request.Scheme);
            return SignOut(new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            }, OpenIdConnectDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}