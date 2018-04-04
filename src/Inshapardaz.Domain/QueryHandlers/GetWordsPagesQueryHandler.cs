using System;
using System.Threading;
using System.Threading.Tasks;
using Paramore.Darker;
using Inshapardaz.Domain.Elasticsearch;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordsPagesQueryHandler : QueryHandlerAsync<GetWordPageQuery, Page<Word>>
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public GetWordsPagesQueryHandler(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public override async Task<Page<Word>> ExecuteAsync(GetWordPageQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(query.DictionaryId);

            var response = await client.SearchAsync<Word>(s => s
                                        .Index(index)
                                        .From(query.PageSize * (query.PageNumber -1))
                                        .Size(query.PageSize)
                                        .Query(q => q.MatchAll())
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

        private static int CalculateTotalPageCount(GetWordPageQuery query, int count)
        {
            return (int)Math.Ceiling((decimal)count / query.PageSize);
        }
    }
}