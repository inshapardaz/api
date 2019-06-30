using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Functions.Library;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Dictionary
{
    public class GetAttributes : FunctionBase
    {
        public GetAttributes(IAmACommandProcessor commandProcessor) 
        : base(commandProcessor)
        {
        }

        [FunctionName("GetAttributes")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "attributes")] HttpRequest req,
            ILogger log)
        {
             var result =  Enum.GetValues(typeof(GrammaticalType))
                .Cast<GrammaticalType>()
                .Select(type => new KeyValuePair<string, int>(Enum.GetName(typeof(GrammaticalType), type), (int)type))
                .ToList();
            return new OkObjectResult(result);
        }

        public static LinkView Self(string relType = RelTypes.Self) => SelfLink("attributes", relType);
    }
}
