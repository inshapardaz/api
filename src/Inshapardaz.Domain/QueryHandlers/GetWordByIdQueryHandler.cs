using System.Linq;
using Paramore.Darker;
using Inshapardaz.Domain.Queries;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Elasticsearch;
using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordByIdQueryHandler : QueryHandlerAsync<GetWordByIdQuery, Word>
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public GetWordByIdQueryHandler(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public override async Task<Word> ExecuteAsync(GetWordByIdQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(query.DictionaryId);

            var existsResponse = await client.IndexExistsAsync(index, cancellationToken: cancellationToken);
            if (!existsResponse.Exists)
            {
                return null;
            }

            var response = await client.SearchAsync<Word>(s => s
                                        .Index(index)
                                        .Size(1)
                                        .Query(q => q
                                            .Bool(b => b
                                            .Must(m => m
                                                .Term(term => term.Field(f => f.Id).Value(query.WordId)))
                                        )), cancellationToken);

            return response.Documents.SingleOrDefault();
        }
    }
}