using System;
using System.IO;
using System.Threading.Tasks;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Dictionary
{
    public class GetDictionaries : FunctionBase
    {
        public GetDictionaries(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator)
            : base(commandProcessor, authenticator)
        {
        }

        [FunctionName("GetDictionaries")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            return new OkObjectResult("GET:Dictionaries");            
        }

        public static LinkView Link(string relType = RelTypes.Self) => SelfLink("authors", relType);
    }
}
