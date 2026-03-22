using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers;

public class HomeController(
    IAmACommandProcessor commandProcessor,
    IQueryProcessor queryProcessor)
    : Controller
{
    private readonly IAmACommandProcessor _commandProcessor = commandProcessor;
    private readonly IQueryProcessor _queryProcessor = queryProcessor;

    [HttpGet("health/check")]
    public IActionResult GetHeathCheck() => Ok();
}

