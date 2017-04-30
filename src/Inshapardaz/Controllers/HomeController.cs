using Inshapardaz.Model;
using Inshapardaz.Renderers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inshapardaz.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRenderResponse<EntryView> _entryRenderer;

        public HomeController(IRenderResponse<EntryView> entryRenderer)
        {
            _entryRenderer = entryRenderer;
        }

        [HttpGet("api", Name = "Entry")]
        public EntryView Index()
        {
            return _entryRenderer.Render();
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
