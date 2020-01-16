﻿using System.Threading.Tasks;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Dictionary.Words.Meanings
{
    public class AddMeaning : FunctionBase
    {
        public AddMeaning(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("AddMeaning")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "dictionaries/{dictionaryId:int}/words/{wordId:long}/meanings")] HttpRequest req,
            int dictionaryId, long wordId,
            ILogger log)
        {
            return new OkObjectResult($"POST:AddMeaning({dictionaryId}, {wordId})");            
        }
    }
}
