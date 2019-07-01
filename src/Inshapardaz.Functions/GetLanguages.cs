using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Inshapardaz.Functions.Views;
using System;
using Inshapardaz.Domain.Entities;
using System.Linq;
using System.Collections.Generic;
using Paramore.Brighter;
using Inshapardaz.Functions.Authentication;

namespace Inshapardaz.Functions
{
    public class GetLanguages : FunctionBase
    {
        public GetLanguages(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator)
            : base(commandProcessor, authenticator)
        {
        }
        
        [FunctionName("GetLanguages")]
        public async Task<IActionResult> Run(
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
