using Darker;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Model;
using Inshapardaz.Renderers;
using Microsoft.AspNetCore.Mvc;

namespace Inshapardaz.Controllers
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
        [Route("api/words/startWith/{title}", Name = "GetWordsListStartWith")]
        public IActionResult StartsWith(string title, int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return NotFound();
            }

            var query = new WordStartingWithQuery
                            {
                                PageSize = pageSize,
                                PageNumber = pageNumber,
                                Title = title
                            };
            var results = _queryProcessor.Execute(query);

            var pageRenderArgs = new PageRendererArgs<Word>()
                                     {
                                         RouteName = "GetWordsListStartWith",
                                         Page = results.Page
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
            var pageRenderArgs = new PageRendererArgs<Word>()
            {
                RouteName = "WordSearch",
                Page = response.Page
            };

            return new ObjectResult(_pageRenderer.Render(pageRenderArgs));
        }

        [HttpGet]
        [Route("api/words/list", Name = "GetWords")]
        public IActionResult Get(int pageNumber = 1, int pageSize = 10)
        {
            var query = new WordQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var results = _queryProcessor.Execute(query);

            var pageRenderArgs = new PageRendererArgs<Word>()
            {
                RouteName = "GetWords",
                Page = results.Page
            };

            return new ObjectResult(_pageRenderer.Render(pageRenderArgs));
        }
    }
}