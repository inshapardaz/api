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
    public class GetMeaningById : FunctionBase
    {
        public GetMeaningById(IAmACommandProcessor commandProcessor)
            : base(commandProcessor)
        {
        }

        [FunctionName("GetMeaningById")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dictionaries/{dictionaryId:int}/words/{wordId:long}/meanings/{meaningId:int}")] HttpRequest req,
            int dictionaryId, long wordId, int meaningId,
            ILogger log)
        {
            return new OkObjectResult($"GET:GetMeaningById({dictionaryId}, {wordId}, {meaningId})");            
        }
    }
}
