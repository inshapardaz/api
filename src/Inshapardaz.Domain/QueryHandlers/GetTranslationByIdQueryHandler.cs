using System.Linq;
using Paramore.Darker;
using Inshapardaz.Domain.Queries;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Elasticsearch;
using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetTranslationByIdQueryHandler : QueryHandlerAsync<GetTranslationByIdQuery, Translation>
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public GetTranslationByIdQueryHandler(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public override async Task<Translation> ExecuteAsync(GetTranslationByIdQuery query,
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
                                       .Term(term => term.Field(f => f.Id).Value(query.WordId)))
                                  )), cancellationToken);

            var word = response.Documents.SingleOrDefault();

            return word?.Translation.SingleOrDefault(t => t.Id == query.TranslationId);
        }
    }
}