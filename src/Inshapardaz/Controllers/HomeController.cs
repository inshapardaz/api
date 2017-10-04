using System.Threading.Tasks;
using Inshapardaz.Api.Adapter;
using Inshapardaz.Api.View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Api.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IActionContextAccessor _actionContextAccessor;

        public HomeController(IAmACommandProcessor commandProcessor, IActionContextAccessor actionContextAccessor)
        {
            _commandProcessor = commandProcessor;
            _actionContextAccessor = actionContextAccessor;
        }

        [HttpGet("api", Name = "Entry")]
        [Produces(typeof(EntryView))]
        public async Task<IActionResult> Index()
        { 
            var command = new GetEntryCommand();
            _commandProcessor.Send(command);
            return Ok(command.Result);
        }
    }
}
