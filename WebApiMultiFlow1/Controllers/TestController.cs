using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApiMultiFlow1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TestController : ControllerBase
    {
        [Authorize(Policy = Constants.UserReadScope)]
        public IActionResult GetData()
        {
            return Ok(new { message = "Hello world! Welcome to Azure B2C" });
        }
    }
}