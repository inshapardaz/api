using System.Threading;
using System.Threading.Tasks;
using Paramore.Darker;
using Inshapardaz.Domain.Elasticsearch;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordStartingWithQueryHandler : QueryHandlerAsync<GetWordStartingWithQuery, Page<Word>>
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public GetWordStartingWithQueryHandler(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public override async Task<Page<Word>> ExecuteAsync(GetWordStartingWithQuery query, CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(query.DictionaryId);

            var response = await client.SearchAsync<Word>(s => s
                                        .Index(index)
                                        .From(query.PageSize * (query.PageNumber - 1))
                                        .Size(query.PageSize)
                                        .Query(q => q
                                        .Prefix(c => c
                                        .Field(f => f.Title)
                                        .Value(query.Title)))
                                    , cancellationToken);

            var words = response.Documents;

            var count = response.HitsMetadata.Total;


            return new Page<Word>
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = count,
                Data = words
            };
        }
    }
}