using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Inshapardaz.Functions.Views;

namespace Inshapardaz.Functions.Dictionaries
{
    public class UpdateDictionary : CommandBase
    {
        public UpdateDictionary(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("UpdateDictionary")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "dictionaries/{dictionaryId:int}")] HttpRequest req,
            int dictionaryId,
            ILogger log)
        {
            return new OkObjectResult($"PUT:UpdateDictionary({dictionaryId})");
        }

        public static LinkView Link(int dictionaryId, string relType = RelTypes.Self) => SelfLink($"dictionaries/{dictionaryId}", relType, "PUT");
    }
}
