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

        [HttpGet("/tools/{language}/spellchecker/punctuation")]
        public async Task<IActionResult> GetPunctuationFixList(string language, CancellationToken cancellationToken)
        {
            var command = new GetPunctuationListCommand() { Language = language };
            await _commandProcessor.SendAsync(command, cancellationToken: cancellationToken);
            return Ok(command.Result);
        }

        [HttpGet("/tools/{language}/spellchecker/autoFix")]
        public async Task<IActionResult> GetAutoFixList(string language, CancellationToken cancellationToken)
        {
            var command = new GetAutoFixListCommand() { Language = language };
            await _commandProcessor.SendAsync(command, cancellationToken: cancellationToken);
            return Ok(command.Result);
        }

        [HttpGet("/tools/{language}/spellchecker/corrections")]
        public async Task<IActionResult> GetCorrectionsList(string language, CancellationToken cancellationToken)
        {
            var command = new GetCorrectionsListCommand() { Language = language };
            await _commandProcessor.SendAsync(command, cancellationToken: cancellationToken);
            return Ok(command.Result);
        }
    }
}
