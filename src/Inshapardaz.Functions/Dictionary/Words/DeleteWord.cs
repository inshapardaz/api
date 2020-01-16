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
    public class DeleteWord : FunctionBase
    {
        public DeleteWord(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("DeleteWord")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "dictionaries/{dictionaryId:int}/words/{wordId:long}")] HttpRequest req,
            int dictionaryId, long wordId,
            ILogger log)
        {
            return new OkObjectResult($"DELETE:DeleteWord({dictionaryId}, {wordId})");            
        }
    }
}
