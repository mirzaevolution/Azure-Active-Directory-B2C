using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using WebAppMultiFlow1.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;

namespace WebAppMultiFlow1.Controllers
{
    [Authorize]
    public class HelloApiController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _baseApiAddress;
        private readonly AzureAdB2COptions _options;
        private readonly IDistributedCache _cache;
        public HelloApiController(IConfiguration configuration,
            IOptions<AzureAdB2COptions> options,
            IDistributedCache cache)
        {
            _configuration = configuration;
            _baseApiAddress = _configuration["ApiEndpoint"];
            _options = options.Value;
            _cache = cache;
        }
        public async Task<IActionResult> ReadAccess()
        {
            using (HttpClient httpClient = new HttpClient())
            {

                string accessToken = await HttpContext.GetTokenAsync("access_token");
                httpClient.BaseAddress = new Uri(_baseApiAddress);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await httpClient.GetAsync("/api/Test");
                if (response.IsSuccessStatusCode)
                {
                    var text = await response.Content.ReadAsStringAsync();
                    return View((object)text);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {

                    return Challenge(new AuthenticationProperties
                    {
                        RedirectUri = Url.Action(nameof(ReadAccess), "HelloApi", null, Request.Scheme)
                    }, User.FindFirstValue("tfp"));
                }
                else
                {
                    return View((object)$"An error occured. Code: {response.StatusCode}");
                }
            }
        }
    }
}