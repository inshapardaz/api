using System.Threading.Tasks;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Dictionaries.Words.Meanings
{
    public class DeleteMeaning : CommandBase
    {
        public DeleteMeaning(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("DeleteMeaning")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "dictionaries/{dictionaryId:int}/words/{wordId:long}/meanings/{meaningId:int}")] HttpRequest req,
            int dictionaryId, long wordId, int meaningId,
            ILogger log)
        {
            return new OkObjectResult($"DELETE:DeleteMeaning({dictionaryId}, {wordId}, {meaningId})");            
        }
    }
}
