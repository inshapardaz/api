using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Inshapardaz.Api.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("health/check")]
        public IActionResult GetHeathCheck()
        {
            return Ok();
        }
    }
}
