using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Inshapardaz.Functions.Views;

namespace Inshapardaz.Functions.Dictionaries.Words
{
    // TODO: Add query string parameter to filter words
    public class GetWordsByDictionary : CommandBase
    {
        public GetWordsByDictionary(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("GetWordsByDictionary")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dictionaries/{dictionaryId:int}/words")] HttpRequest req,
            int dictionaryId,
            ILogger log)
        {
            return new OkObjectResult($"GET:GetWordsByDictionary({dictionaryId})");
        }

        public static LinkView Link(int dictionaryId, string relType = RelTypes.Self) => SelfLink($"dictionaries/{dictionaryId}/words", relType, "GET");
    }
}
