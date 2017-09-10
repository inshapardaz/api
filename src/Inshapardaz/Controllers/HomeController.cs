using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
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
        public IActionResult Index()
        {
            return Ok(_entryRenderer.Render());
        }
    }
}
