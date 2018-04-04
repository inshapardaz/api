using System.Linq;
using System.Threading.Tasks;
using Inshapardaz.Api.Adapters;
using Inshapardaz.Api.View;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;

        public HomeController(IAmACommandProcessor commandProcessor, IActionContextAccessor actionContextAccessor)
        {
            _commandProcessor = commandProcessor;
        }

        [HttpGet("api", Name = "Entry")]
        [Produces(typeof(EntryView))]
        public async Task<IActionResult> Index()
        { 
            var command = new GetEntryRequest();
            await _commandProcessor.SendAsync(command);
            return Ok(command.Result);
        }

        [HttpGet("auth")]
        [Authorize]
        public IActionResult Auth(){
            return new JsonResult(User.Claims.Select(c => new { c.Type, c.Value }));
        }
    }
}
