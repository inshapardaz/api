using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Paramore.Brighter;
using Inshapardaz.Functions.Views;
using System.Threading;
using Inshapardaz.Functions.Authentication;
using System.Security.Claims;
using Inshapardaz.Functions.Views.Dictionaries;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Dictionaries;
using Inshapardaz.Functions.Mappings;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Extensions;

namespace Inshapardaz.Functions.Dictionaries
{
    public class UpdateDictionary : CommandBase
    {
        public UpdateDictionary(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("UpdateDictionary")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "dictionaries/{dictionaryId:int}")] HttpRequest req,
            int dictionaryId,
            [AccessToken] ClaimsPrincipal principal,
            CancellationToken token)
        {
            if (principal == null)
            {
                return new UnauthorizedResult();
            }

            if (!principal.IsWriter())
            {
                return new ForbidResult("Bearer");
            }

            var dictionary = await GetBody<DictionaryEditView>(req);

            var request = new UpdateDictionaryRequest(dictionaryId, dictionary.Map());
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = request.Result.Dictionary.Render(principal);

            if (request.Result.HasAddedNew)
            {
                return new CreatedResult(renderResult.Links.Self(), renderResult);
            }

            return new OkObjectResult(renderResult);
        }

        public static LinkView Link(int dictionaryId, string relType = RelTypes.Self) => SelfLink($"dictionaries/{dictionaryId}", relType, "PUT");
    }
}
