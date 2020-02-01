using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Paramore.Brighter;
using System.Security.Claims;
using Inshapardaz.Functions.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Ports.Dictionaries;
using Inshapardaz.Functions.Mappings;
using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Views.Dictionaries;
using Inshapardaz.Functions.Views;

namespace Inshapardaz.Functions.Dictionaries
{
    public class AddDictionary : CommandBase
    {
        public AddDictionary(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("AddDictionary")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "dictionaries")] HttpRequest req,
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

            var request = new AddDictionaryRequest(principal.GetUserId(), dictionary.Map());
            await CommandProcessor.SendAsync(request, cancellationToken: token);

            var renderResult = request.Result.Render(principal);
            return new CreatedResult(renderResult.Links.Self(), renderResult);
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink($"dictionaries", relType, "POST");
    }
}
