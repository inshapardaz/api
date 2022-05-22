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
