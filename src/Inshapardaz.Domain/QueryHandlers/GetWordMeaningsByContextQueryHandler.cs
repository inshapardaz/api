using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Paramore.Darker;
using Inshapardaz.Domain.Elasticsearch;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordMeaningsByContextQueryHandler : QueryHandlerAsync<GetWordMeaningsByContextQuery, IEnumerable<Meaning>>
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public GetWordMeaningsByContextQueryHandler(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public override async Task<IEnumerable<Meaning>> ExecuteAsync(GetWordMeaningsByContextQuery query,
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
            if (word != null)
            {
                if (string.IsNullOrWhiteSpace(query.Context))
                {
                    return word.Meaning;
                }

                return word.Meaning.Where(m => m.Context == query.Context);
            }

            return new Meaning[0];
        }
    }
}