using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAppMultiFlow2.Models;

namespace WebAppMultiFlow2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> AllClaims()
        {
            List<AuthTokenData> list = new List<AuthTokenData>();
            string accessToken = await HttpContext.GetTokenAsync("access_token");

            if (User.Claims != null && User.Claims.Count() > 0)
            {
                foreach (var item in User.Claims)
                {
                    list.Add(new AuthTokenData
                    {
                        Key = item.Type,
                        Value = item.Value
                    });
                }
            }
            return View(list);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
