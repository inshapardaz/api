using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Elasticsearch;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordsForTranslationsByLanguageQueryHandler : QueryHandlerAsync<GetWordsForTranslationsByLanguageQuery, Dictionary<string, Word>>
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public GetWordsForTranslationsByLanguageQueryHandler(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public override async Task<Dictionary<string, Word>> ExecuteAsync(GetWordsForTranslationsByLanguageQuery query,
                                                                                  CancellationToken cancellationToken = default(CancellationToken))
        {
            //var client = _clientProvider.GetClient();
            //var index = _indexProvider.GetIndexForDictionary(query.DictionaryId);

            //var response = await client.SearchAsync<Word>(s => s
            //                            .Index(index)
            //                            .Size(1)
            //                            .Query(q => q
            //                                .Bool(b => b
            //                                .Should(h =>
            //                                h.Term(term => term.Title, query.Words) && 
            //                                h.Term(t2 => t2.))
            //                        )), cancellationToken);

            //var word = response.Documents.SingleOrDefault();
            //return word?.Translation;
            //List<Translation> translations;
            //if (query.IsTranspiling.HasValue)
            //{
            //    translations = await _database.Translation
            //                          .Where(x => x.Language == query.Language &&
            //                                      x.IsTrasnpiling == query.IsTranspiling &&
            //                                      query.Words.Any(w => w == x.Value))
            //                                      .ToListAsync(cancellationToken);
            //}

            //else
            //{
            //    translations = await _database.Translation
            //                          .Where(x => x.Language == query.Language &&
            //                                      x.IsTrasnpiling == query.IsTranspiling &&
            //                                      query.Words.Any(w => w == x.Value))
            //                          .ToListAsync(cancellationToken);
            //}

            //var result = new Dictionary<string,Word>();
            //foreach (var translation in translations)
            //{
            //    if (result.ContainsKey(translation.Value))
            //    {
            //        // IMPORVE : We have mulitple words for this translation. 
            //        continue;
            //    }
            //    else
            //    {
            //        var word = _database.Word.SingleOrDefault(w => w.Id == translation.WordId);
            //        result.Add(translation.Value, word);
            //    }
            //}

            //return result;
            throw new NotImplementedException();
        }
    }
}