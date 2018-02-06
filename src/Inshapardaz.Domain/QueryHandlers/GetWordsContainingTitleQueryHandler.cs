using System.Threading;
using System.Threading.Tasks;
using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Elasticsearch;
using Inshapardaz.Domain.Queries;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordsContainingTitleQueryHandler : QueryHandlerAsync<GetWordContainingTitleQuery, Page<Word>>
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public GetWordsContainingTitleQueryHandler(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public override async Task<Page<Word>> ExecuteAsync(GetWordContainingTitleQuery query, CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(query.DictionaryId);

            var response = await client.SearchAsync<Word>(s => s
                                        .Index(index)
                                        .From(query.PageSize * (query.PageNumber - 1))
                                        .Size(query.PageSize)
                                        .Query(q => q
                                        .Bool(b => b
                                        .Must(m => m
                                        .Term(term => term.Field(f => f.Id).Value(query.SearchTerm)))
                                    )), cancellationToken);

            var words = response.Documents;

            var count = response.HitsMetadata.Hits.Count;


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