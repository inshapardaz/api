using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.LanguageUtility
{
    public class ThesaurusRequest : IRequest
    {
        public Guid Id { get; set; }
        public int PageSize { get; internal set; }
        public int PageNumber { get; internal set; }
        public string Word { get; internal set; }
        public PageView<WordView> Response { get; internal set; }
    }

    public class ThesaurusRequestHandler : RequestHandlerAsync<ThesaurusRequest>
    {
        private readonly IWordRepository _wordRepository;
        private readonly IRenderWordPage _pageRenderer;

        protected ThesaurusRequestHandler(IWordRepository wordRepository, IRenderWordPage pageRenderer)
        {
            _wordRepository = wordRepository;
            _pageRenderer = pageRenderer;
        }

        public async override Task<ThesaurusRequest> HandleAsync(ThesaurusRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            //var query = new GetWordRelationsByTitleQuery
            //{
            //    PageSize = request.PageSize,
            //    PageNumber = request.PageNumber,
            //    Title = request.Word
            //};
            //var results = await _wordRepository.GetWordsByTitles(query, cancellationToken);
            //var pageRenderArgs = new PageRendererArgs<Word>
            //{
            //    RouteName = "GetWordAlternatives",
            //    Page = results
            //};

            //request.Response = _pageRenderer.Render(pageRenderArgs, -1);

            throw new NotImplementedException();
            return await base.HandleAsync(request, cancellationToken);
        }
    }
}
