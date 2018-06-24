using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Library
{
    public class GetAuthorsRequest : RequestBase
    {
        public GetAuthorsRequest(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public PageView<AuthorView> Result { get; set; }
    }

    public class GetAuthorsRequestHandler : RequestHandlerAsync<GetAuthorsRequest>
    {
        private readonly IUserHelper _userHelper;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderDictionaries _dictionariesRenderer;

        public GetAuthorsRequestHandler(IQueryProcessor queryProcessor, IUserHelper userHelper, IRenderDictionaries dictionariesRenderer)
        {
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _dictionariesRenderer = dictionariesRenderer;
        }

        public override async Task<GetAuthorsRequest> HandleAsync(GetAuthorsRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            /*var userId = _userHelper.GetUserId();

            var results = await _queryProcessor.ExecuteAsync(new GetDictionariesByUserQuery {UserId = userId}, cancellationToken);

            var wordCounts = new Dictionary<int, int>();
            var downloads = new Dictionary<int, IEnumerable<DictionaryDownload>>();
            foreach (var dictionary in results)
            {
                var wordCount = await _queryProcessor.ExecuteAsync(new GetDictionaryWordCountQuery
                {
                    DictionaryId = dictionary.Id
                }, cancellationToken);
                wordCounts.Add(dictionary.Id, wordCount);

            command.Result = _dictionariesRenderer.Render(results, wordCounts, downloads);*/
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
