using System.Threading.Tasks;
using Inshapardaz.Api.Adapters.Dictionary;
using Inshapardaz.Api.View;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers
{
    public class SearchController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;

        public SearchController(IAmACommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        [HttpGet("api/dictionaries/{id}/Search", Name = "SearchDictionary")]
        [Produces(typeof(PageView<WordView>))]
        public async Task<IActionResult> SearchDictionary(int id, string query, int pageNumber = 1, int pageSize = 10)
        {
            var request = new SearchWordRequest
            {
                DictionaryId = id,
                Query = query,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            await _commandProcessor.SendAsync(request);
            
            return Ok(request.Result);
        }

        [HttpGet("api/dictionaries/{id}/words/startWith/{startingWith}", Name = "GetWordsListStartWith")]
        [Produces(typeof(PageView<WordView>))]
        public async Task<IActionResult> StartsWith(int id, string startingWith, int pageNumber = 1, int pageSize = 10)
        {
            var request = new GetWordsStartingWithRequest
            {
                DictionaryId = id,
                StartingWith = startingWith,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            await _commandProcessor.SendAsync(request);

            return Ok(request.Result);
        }
    }
}