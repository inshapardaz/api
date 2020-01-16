using System.Threading.Tasks;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Dictionary.Words.Translations
{
    public class UpdateTranslation : FunctionBase
    {
        public UpdateTranslation(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("UpdateTranslation")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "dictionaries/{dictionaryId:int}/words/{wordId:long}/translations/{translationId:int}")] HttpRequest req,
            int dictionaryId, long wordId, int translationId,
            ILogger log)
        {
            return new OkObjectResult($"PUT:UpdateTranslation({dictionaryId}, {wordId}, {translationId})");            
        }
    }
}
