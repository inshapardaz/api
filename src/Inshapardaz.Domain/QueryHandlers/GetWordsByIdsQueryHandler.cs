using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Paramore.Darker;
using Inshapardaz.Domain.Elasticsearch;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Nest;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordsByIdsQueryHandler : QueryHandlerAsync<GetWordsByIdsQuery, Page<Word>>
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public GetWordsByIdsQueryHandler(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public override async Task<Page<Word>> ExecuteAsync(GetWordsByIdsQuery query, CancellationToken cancellationToken)
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
                                            .Term(term => AddIds(term.Field(f => f.Id), query.IDs)))
                                                    )), cancellationToken);

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

        private TermQueryDescriptor<Word> AddIds(TermQueryDescriptor<Word> field, IEnumerable<long> queryDs)
        {
            foreach (var id in queryDs)
            {
                field.Value(id);
            }

            return field;
        }
    }
}