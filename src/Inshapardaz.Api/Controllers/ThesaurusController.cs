using Paramore.Darker;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Inshapardaz.Api.Controllers
{
    public class ThesaurusController : Controller
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderWordPage _pageRenderer;

        public ThesaurusController(
            IQueryProcessor queryProcessor,
            IRenderWordPage pageRenderer)
        {
            _queryProcessor = queryProcessor;
            _pageRenderer = pageRenderer;
        }

        [HttpGet("api/alternates/{word}", Name = "GetWordAlternatives")]
        public IActionResult Get(string word, int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return NotFound();
            }

            var query = new GetWordRelationsByTitleQuery
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
                Title = word
            };
            var results = _queryProcessor.Execute(query);
            var pageRenderArgs = new PageRendererArgs<Word>
            {
                RouteName = "GetWordsListStartWith",
                Page = results
            };

            return new ObjectResult(_pageRenderer.Render(pageRenderArgs, -1));
        }
    }
}
