using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Dictionaries
{
    public class GetDictionaryById : CommandBase
    {
        public GetDictionaryById(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("GetDictionaryById")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dictionaries/{dictionaryId:int}")] HttpRequest req,
            int dictionaryId,
            ILogger log)
        {
            return new OkObjectResult($"GET:DictionaryById({dictionaryId})");            
        }
    }
}
