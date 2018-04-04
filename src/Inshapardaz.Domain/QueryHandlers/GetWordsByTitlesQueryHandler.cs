using Paramore.Darker;
using Inshapardaz.Domain.Queries;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Inshapardaz.Domain.Elasticsearch;
using Inshapardaz.Domain.Entities;
using Nest;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordsByTitlesQueryHandler : QueryHandlerAsync<GetWordsByTitlesQuery, IEnumerable<Word>>
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public GetWordsByTitlesQueryHandler(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public override async Task<IEnumerable<Word>> ExecuteAsync(GetWordsByTitlesQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(query.DictionaryId);

            var response = await client.SearchAsync<Word>(s => s
                                                               .Index(index)
                                                               .Size(100)
                                                               .Query(q => q
                                                               .Bool(b => b
                                                                .Must(m => m
                                                                    .Term(term => AddValues(term.Field(f => f.Id), query.Titles)))
                                                            )), cancellationToken);

            return response.Documents;
        }

        private TermQueryDescriptor<Word> AddValues(TermQueryDescriptor<Word> field, IEnumerable<string> queryDs)
        {
            foreach (var id in queryDs)
            {
                field.Value(id);
            }

            return field;
        }
    }
}