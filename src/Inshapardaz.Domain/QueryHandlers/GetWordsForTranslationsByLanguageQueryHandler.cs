using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Microsoft.EntityFrameworkCore;
using Paramore.Darker;
using Translation = Inshapardaz.Domain.Database.Entities.Translation;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordsForTranslationsByLanguageQueryHandler : QueryHandlerAsync<GetWordsForTranslationsByLanguageQuery, Dictionary<string, Word>>
    {
        private readonly IDatabaseContext _database;

        public GetWordsForTranslationsByLanguageQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<Dictionary<string, Word>> ExecuteAsync(GetWordsForTranslationsByLanguageQuery query,
                                                                                  CancellationToken cancellationToken = default(CancellationToken))
        {
            List<Translation> translations;
            if (query.IsTranspiling.HasValue)
            {
                translations = await _database.Translation
                                      .Where(x => x.Language == query.Language &&
                                                  x.IsTrasnpiling == query.IsTranspiling &&
                                                  query.Words.Any(w => w == x.Value))
                                                  .ToListAsync(cancellationToken);
            }

            else
            {
                translations = await _database.Translation
                                      .Where(x => x.Language == query.Language &&
                                                  x.IsTrasnpiling == query.IsTranspiling &&
                                                  query.Words.Any(w => w == x.Value))
                                      .ToListAsync(cancellationToken);
            }

            var result = new Dictionary<string,Word>();
            foreach (var translation in translations)
            {
                if (result.ContainsKey(translation.Value))
                {
                    // IMPORVE : We have mulitple words for this translation. 
                    continue;
                }
                else
                {
                    var word = _database.Word.SingleOrDefault(w => w.Id == translation.WordId);
                    result.Add(translation.Value, word);
                }
            }

            return result;
        }
    }
}