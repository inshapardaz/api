using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inshapardaz.Api.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRenderResponse<EntryView> _entryRenderer;

        public HomeController(IRenderResponse<EntryView> entryRenderer)
        {
            _entryRenderer = entryRenderer;
        }

        [HttpGet("api", Name = "Entry")]
        [Produces(typeof(EntryView))]
        [Authorize]
        public IActionResult Index()
        {
            return Ok(_entryRenderer.Render());
        }

        [Authorize]
        [HttpGet]
        [Route("api/test")]
        public string Test()
        {
            return "All good. You only get this message if you are authenticated.";
        }
    }
}
