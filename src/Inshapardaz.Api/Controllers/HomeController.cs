using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Adapters;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Repositories.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IChapterRepository _chapterRepository;

        public HomeController(IAmACommandProcessor commandProcessor, IActionContextAccessor actionContextAccessor, IChapterRepository chapterRepository)
        {
            _commandProcessor = commandProcessor;
            _chapterRepository = chapterRepository;
        }

        [HttpGet("/")]
        public IActionResult HomeIndex()
        {
            return RedirectToAction("Index");
        }

        [HttpGet("api", Name = "Entry")]
        [Produces(typeof(EntryView))]
        public async Task<IActionResult> Index()
        { 
            var command = new GetEntryRequest();
            await _commandProcessor.SendAsync(command);
            return Ok(command.Result);
        }

        [HttpGet("doit")]
        public async Task<IActionResult> DoIt(CancellationToken cancellationToken)
        {
            await _chapterRepository.MigrateContents(cancellationToken);
            return Ok();
        }
    }
}
