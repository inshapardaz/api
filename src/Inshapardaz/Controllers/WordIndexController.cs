using System;
using Darker;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Inshapardaz.Api.Controllers
{
    public class WordIndexController : Controller
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderResponseFromObject<PageRendererArgs<Word>, PageView<WordView>> _pageRenderer;

        public WordIndexController(
            IQueryProcessor queryProcessor,
            IRenderResponseFromObject<PageRendererArgs<Word>, PageView<WordView>> pageRenderer)
        {
            _queryProcessor = queryProcessor;
            _pageRenderer = pageRenderer;
        }

        [HttpGet]
        [Route("api/dictionaries/{id}/Search", Name = "SearchDictionary")]
        public IActionResult SearchDictionary(int id, string query, int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return NotFound();
            }

            var wordQuery = new WordContainingTitleQuery
            {
                DictionaryId = id,
                SearchTerm = query,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var response = _queryProcessor.Execute(wordQuery);
            var pageRenderArgs = new PageRendererArgs<Word>
            {
                RouteName = "SearchDictionary",
                Page = response,
                RouteArguments = new DictionarSearchPageRouteArgs
                {
                    Id = id,
                    Query = query
                }
            };

            return new ObjectResult(_pageRenderer.Render(pageRenderArgs));
        }

        [HttpGet]
        [Route("api/dictionaries/{id}/words/startWith/{startingWith}", Name = "GetWordsListStartWith")]
        public IActionResult StartsWith(int id, string startingWith, int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(startingWith))
            {
                return NotFound();
            }

            var query = new WordStartingWithQuery
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
                Title = startingWith,
                DictionaryId = id
            };
            var results = _queryProcessor.Execute(query);

            var pageRenderArgs = new PageRendererArgs<Word>
            {
                RouteName = "GetWordsListStartWith",
                Page = results,
                RouteArguments = new RouteWithTitlePageRouteArgs
                {
                    Title = startingWith
                }
            };

            return new ObjectResult(_pageRenderer.Render(pageRenderArgs));
        }

        [Route("api/words/search/{title}", Name = "WordSearch")]
        [HttpGet]
        public IActionResult Search(string title, int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return NotFound();
            }

            var query = new WordContainingTitleQuery
            {
                SearchTerm = title,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var response = _queryProcessor.Execute(query);
            var pageRenderArgs = new PageRendererArgs<Word>
            {
                RouteName = "WordSearch",
                Page = response,
                RouteArguments = new RouteWithTitlePageRouteArgs
                {
                    Title = title
                }
            };

            return new ObjectResult(_pageRenderer.Render(pageRenderArgs));
        }
    }
}