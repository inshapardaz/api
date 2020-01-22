using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Inshapardaz.Functions.Views;

namespace Inshapardaz.Functions.Dictionaries
{
    public class GetDictionaryDownloadById : CommandBase
    {
        public GetDictionaryDownloadById(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("GetDictionaryDownloadById")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dictionaries/{dictionaryId:int}/download")] HttpRequest req,
            int dictionaryId,
            ILogger log)
        {
            return new OkObjectResult($"GET:DictionaryDownloadById({dictionaryId})");
        }

        public static LinkView Link(int dictionaryId, string mimeType, string relType = RelTypes.Self) => SelfLink($"dictionaries/{dictionaryId}/download", relType, "GET", media: mimeType);
    }
}
