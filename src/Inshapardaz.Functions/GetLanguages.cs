using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using Inshapardaz.Domain.Models;
using System.Linq;
using System.Collections.Generic;
using Paramore.Brighter;
using Inshapardaz.Functions.Views;

namespace Inshapardaz.Functions
{
    public class GetLanguages : CommandBase
    {
        public GetLanguages(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }
        
        [FunctionName("GetLanguages")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "languages")] HttpRequest req,
            ILogger log)
        {
             var result = Enum.GetValues(typeof(Languages))
                .Cast<Languages>()
                .Select(lang => new KeyValuePair<string, int>(Enum.GetName(typeof(Languages), lang), (int)lang))
                .ToList();
            return new OkObjectResult(result);
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("languages", relType);
    }
}
