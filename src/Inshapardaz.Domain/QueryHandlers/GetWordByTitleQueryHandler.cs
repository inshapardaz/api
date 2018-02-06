using System.Linq;
using Paramore.Darker;
using Inshapardaz.Domain.Queries;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Elasticsearch;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordByTitleQueryHandler : QueryHandlerAsync<GetWordByTitleQuery, Word>
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public GetWordByTitleQueryHandler(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public override async Task<Word> ExecuteAsync(GetWordByTitleQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(query.DictionaryId);

            var response = await client.SearchAsync<Word>(s => s
                                        .Index(index)
                                        .Size(1)
                                        .Query(q => q
                                            .Bool(b => b
                                            .Must(m => m
                                                .Term(term => term.Field(f => f.Title).Value(query.Title)))
                            )), cancellationToken);

            return response.Documents.FirstOrDefault();
        }
    }
}