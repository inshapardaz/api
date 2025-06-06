using Inshapardaz.Domain.Ports.Command.Library.Book.Chapter;
using Inshapardaz.Domain.Ports.Query.Library.Book.Chapter;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers;

public class HomeController : Controller
{
    private readonly IAmACommandProcessor _commandProcessor;
    private readonly IQueryProcessor _queryProcessor;

    public HomeController(IAmACommandProcessor commandProcessor,
        IQueryProcessor queryProcessor)
    {
        _commandProcessor = commandProcessor;
        _queryProcessor = queryProcessor;
    }

    [HttpGet("health/check")]
    public IActionResult GetHeathCheck()
    {
        return Ok();
    }
}

