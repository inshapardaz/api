using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Dictionaries.Words.Details
{
    public class UpdateWordDetail : CommandBase
    {
        public UpdateWordDetail(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("UpdateWordDetail")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "dictionaries/{dictionaryId:int}/words/{wordId:long}/details/{detailId:int}")] HttpRequest req,
            int dictionaryId, long wordId, int detailId,
            ILogger log)
        {
            return new OkObjectResult($"PUT:UpdateWordDetail({dictionaryId}, {wordId}, {detailId})");            
        }
    }
}
