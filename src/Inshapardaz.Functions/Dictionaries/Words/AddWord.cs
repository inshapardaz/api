using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Inshapardaz.Functions.Views;

namespace Inshapardaz.Functions.Dictionaries.Words
{
    public class AddWord : CommandBase
    {
        public AddWord(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("AddWord")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "dictionaries/{dictionaryId:int}/words")] HttpRequest req,
            int dictionaryId,
            ILogger log)
        {
            return new OkObjectResult($"POST:AddWord({dictionaryId})");
        }

        public static LinkView Link(int dictionaryId, string relType = RelTypes.Self) => SelfLink($"dictionaries/{dictionaryId}/words", relType, "POST");
    }
}
