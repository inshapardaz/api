using Inshapardaz.Domain.Adapters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Inshapardaz.Api.Controllers
{
    public class HomeController : Controller
    {
        private readonly Settings _setting;

        public HomeController(Settings setting)
        {
            _setting = setting;
        }

        [HttpGet("health/check")]
        public IActionResult GetHeathCheck()
        {
            return Ok();
        }

        [HttpGet("health/config")]
        public IActionResult GetConfig()
        {
            return Ok(JsonConvert.SerializeObject(_setting));
        }
    }
}
