using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Elasticsearch;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Nest;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetTranslationsForWordsByLanguageQueryHandler : QueryHandlerAsync<GetTranslationsForWordsByLanguageQuery, Dictionary<string, Translation>>
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public GetTranslationsForWordsByLanguageQueryHandler(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public override async Task<Dictionary<string, Translation>> ExecuteAsync(GetTranslationsForWordsByLanguageQuery query,
                                                                          CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(query.DictionaryId);

            var response = await client.SearchAsync<Word>(s => s
                                        .Index(index)
                                        .Size(50)
                                        .Query(q => q
                                        .Bool(b => b
                                        .Must(m => m
                                        .Term(term => AddIds(term.Field(f => f.Title), query.Words)))
                                )), cancellationToken);

            var words = response.Documents.ToList();

            var result = new Dictionary<string, Translation>();
            foreach (var word in words)
            {
                if (result.ContainsKey(word.Title))
                {
                    // IMPORVE : We have mulitple translations for this word. 
                    continue;
                }

                Translation translation;
                if (query.IsTranspiling.HasValue)
                {
                    translation = word.Translation.FirstOrDefault(t => t.Language == query.Language && 
                                                                      t.IsTrasnpiling == query.IsTranspiling);
                }
                else
                {
                    translation = word.Translation.FirstOrDefault(t => t.Language == query.Language);
                }

                if (translation != null)
                {
                    result.Add(word.Title, translation);
                }
            }

            return result;
        }

        private TermQueryDescriptor<Word> AddIds(TermQueryDescriptor<Word> field, IEnumerable<string> querys)
        {
            foreach (var id in querys)
            {
                field.Value(id);
            }

            return field;
        }
    }
}