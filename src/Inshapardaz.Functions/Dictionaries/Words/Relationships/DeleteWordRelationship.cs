using System.Threading.Tasks;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Dictionaries.Words.Relationships
{
    public class DeleteWordRelationship : CommandBase
    {
        public DeleteWordRelationship(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("DeleteWordRelationship")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "dictionaries/{dictionaryId:int}/words/{wordId:long}/relationships/{relationshipId:int}")] HttpRequest req,
            int dictionaryId, long wordId, int relationshipId,
            ILogger log)
        {
            return new OkObjectResult($"DELETE:DeleteWordRelationship({dictionaryId}, {wordId}, {relationshipId})");            
        }
    }
}
