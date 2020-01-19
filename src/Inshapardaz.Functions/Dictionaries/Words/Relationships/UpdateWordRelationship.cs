using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Dictionaries.Words.Relationships
{
    public class UpdateWordRelationship : CommandBase
    {
        public UpdateWordRelationship(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("UpdateWordRelationship")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "dictionaries/{dictionaryId:int}/words/{wordId:long}/relationships/{relationshipId:int}")] HttpRequest req,
            int dictionaryId, long wordId, int relationshipId,
            ILogger log)
        {
            return new OkObjectResult($"PUT:UpdateWordRelationship({dictionaryId}, {wordId}, {relationshipId})");            
        }
    }
}
