using Inshapardaz.Domain.Ports.Handlers;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Controllers
{
    public class ToolController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;

        public ToolController(IAmACommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        [HttpPost("/tools/autofix")]
        public async Task<IActionResult> AutoFix([FromBody] string input, CancellationToken cancellationToken)
        {
            var command = new AutoCorrectTextCommand() { Language = "ur", TextToCorrect = input };
            await _commandProcessor.SendAsync(command, cancellationToken: cancellationToken);
            return Ok(command.Result);
        }
    }
}
