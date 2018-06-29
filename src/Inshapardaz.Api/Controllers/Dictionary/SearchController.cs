using System;
using System.Threading.Tasks;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Ports.Dictionary;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace Inshapardaz.Api.Controllers.Dictionary
{
    public class SearchController : Controller
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IRenderWordPage _wordPageRenderer;

        public SearchController(IAmACommandProcessor commandProcessor, IRenderWordPage wordPageRenderer)
        {
            _commandProcessor = commandProcessor;
            _wordPageRenderer = wordPageRenderer;
        }

        [HttpGet("api/dictionaries/Search", Name = "SearchAllDictionaries")]
        [Produces(typeof(PageView<WordView>))]
        public async Task<IActionResult> SearchDictionary(string query, int pageNumber = 1, int pageSize = 10)
        {
            //var request = new SearchWordRequest(id)
            //{
            //    Query = query,
            //    PageNumber = pageNumber,
            //    PageSize = pageSize
            //};
            //await _commandProcessor.SendAsync(request);

            //return Ok(request.Result);
            return NotFound();
        }

        [HttpGet("api/dictionaries/{id}/Search", Name = "SearchDictionary")]
        [Produces(typeof(PageView<WordView>))]
        public async Task<IActionResult> SearchDictionary(int id, string query, int pageNumber = 1, int pageSize = 10)
        {
            var request = new SearchWordRequest(id, query, pageNumber, pageSize);
            await _commandProcessor.SendAsync(request);

            var pageRendererArgs = new PageRendererArgs<Word>
            {
                RouteName = "SearchDictionary",
                Page = request.Result,
                RouteArguments = new PagedRouteArgs
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    DictionaryId = id
                }
            };

            _wordPageRenderer.Render(pageRendererArgs, id);

            return Ok(request.Result);
        }

        [HttpGet("api/dictionaries/{id}/words/startWith/{startingWith}", Name = "GetWordsListStartWith")]
        [Produces(typeof(PageView<WordView>))]
        public async Task<IActionResult> StartsWith(int id, string startingWith, int pageNumber = 1, int pageSize = 10)
        {
            var request = new GetWordsStartingWithRequest(id, startingWith, pageNumber, pageSize);
            await _commandProcessor.SendAsync(request);

            var pageRendererArgs = new PageRendererArgs<Word>
            {
                RouteName = "GetWordsListStartWith",
                Page = request.Result,
                RouteArguments = new PagedRouteArgs
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    DictionaryId = id
                }
            };

            _wordPageRenderer.Render(pageRendererArgs, id);
            return Ok(request.Result);
        }
    }
}