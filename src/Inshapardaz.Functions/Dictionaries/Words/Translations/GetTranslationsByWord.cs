using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Dictionaries.Words.Translations
{
    public class GetTranslationsByWord : CommandBase
    {
        public GetTranslationsByWord(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("GetTranslationsByWord")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dictionaries/{dictionaryId:int}/words/{wordId:long}/translations")] HttpRequest req,
            int dictionaryId, long wordId,
            ILogger log)
        {
            return new OkObjectResult($"GET:GetTranslationsByWord({dictionaryId}, {wordId})");            
        }
    }
}
