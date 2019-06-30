using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Dictionary
{
    public class GetRelationTypes : FunctionBase
    {
        public GetRelationTypes(IAmACommandProcessor commandProcessor) 
        : base(commandProcessor)
        {
        }

        [FunctionName("GetRelationTypes")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "relationships")] HttpRequest req,
            ILogger log)
        {
            var result =  Enum.GetValues(typeof(RelationType))
                .Cast<RelationType>()
                .Select(relation => new KeyValuePair<string, int>(Enum.GetName(typeof(RelationType), relation), (int)relation))
                .ToList();
            return new OkObjectResult(result);
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("relationships", relType);
    }
}
