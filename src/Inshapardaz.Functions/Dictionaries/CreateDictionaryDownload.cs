using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Inshapardaz.Functions.Views;

namespace Inshapardaz.Functions.Dictionaries.Words
{
    public class CreateDictionaryDownload : CommandBase
    {
        public CreateDictionaryDownload(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("CreateDictionaryDownload")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "dictionaries/{dictionaryId:int}/download")] HttpRequest req,
            int dictionaryId,
            ILogger log)
        {
            return new OkObjectResult($"POST:CreateDictionaryDownload({dictionaryId})");
        }

        public static LinkView Link(int dictionaryId, string relType = RelTypes.Self) => SelfLink($"dictionaries/{dictionaryId}/download", relType, "POST");
    }
}
