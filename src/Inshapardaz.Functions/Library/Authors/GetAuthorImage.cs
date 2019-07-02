using System.Threading.Tasks;
using Inshapardaz.Functions.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Library.Authors
{
    public class GetAuthorImage : FunctionBase
    {
        public GetAuthorImage(IAmACommandProcessor commandProcessor, IFunctionAppAuthenticator authenticator) 
        : base(commandProcessor, authenticator)
        {
        }

        [FunctionName("GetAuthorImage")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "authors/{id}/image")] HttpRequest req,
            ILogger log, int id)
        {
            return new OkObjectResult($"Get:Author {id} Image");
        }
    }
}
