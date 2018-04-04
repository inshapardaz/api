using System.Linq;
using Inshapardaz.Domain.Queries;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Elasticsearch;
using Inshapardaz.Domain.Entities;
using Nest;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionaryByIdQueryHandler : QueryHandlerAsync<GetDictionaryByIdQuery, Dictionary>
    {
        private readonly IClientProvider _clientProvider;

        public GetDictionaryByIdQueryHandler(IClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public override async Task<Dictionary> ExecuteAsync(GetDictionaryByIdQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = _clientProvider.GetClient();

            ISearchResponse<Dictionary> response = 
                await client.SearchAsync<Dictionary>(s => s
                                .Index(Indexes.Dictionaries)
                                .Size(1)
                                .Query(q => q
                                .Bool(b => b
                                .Must(m => m
                                .Term(term => term.Field(f => f.Id).Value(query.DictionaryId)))
                            )), cancellationToken);

            return response.Documents.SingleOrDefault();
        }
    }
}