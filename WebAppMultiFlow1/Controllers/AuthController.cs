using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
namespace WebAppMultiFlow1.Controllers
{
    public class AuthController : Controller
    {

        public IActionResult Login(string returnUrl = "/")
        {
            if (returnUrl.Equals("/"))
            {
                returnUrl = Url.Action("Index", "Home", null, Request.Scheme);
            }
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = returnUrl
            }, Constants.SignInSignUpPolicy);
        }

        [Authorize]
        public IActionResult EditProfile()
        {
            string returnUrl = Url.Action("Index", "Home", null, Request.Scheme);
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = returnUrl
            }, Constants.EditProfilePolicy);
        }

        [Authorize]
        public IActionResult Logout()
        {
            string returnUrl = Url.Action("Index", "Home", null, Request.Scheme);
            return SignOut(new AuthenticationProperties
            {
                RedirectUri = returnUrl
            }, CookieAuthenticationDefaults.AuthenticationScheme, Constants.SignInSignUpPolicy, Constants.EditProfilePolicy);
        }
    }
}