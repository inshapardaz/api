using System.Threading.Tasks;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Dictionary.Words
{
    public class GetWordById : FunctionBase
    {
        public GetWordById(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("GetWordById")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dictionaries/{dictionaryId:int}/words/{wordId:long}")] HttpRequest req,
            int dictionaryId, long wordId,
            ILogger log)
        {
            return new OkObjectResult($"GET:GetWordById({dictionaryId}, {wordId})");            
        }
    }
}
