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

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetTranslationsForWordsByLanguageQueryHandler : QueryHandlerAsync<GetTranslationsForWordsByLanguageQuery, Dictionary<string, Translation>>
    {
        private readonly IDatabaseContext _database;

        public GetTranslationsForWordsByLanguageQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<Dictionary<string, Translation>> ExecuteAsync(GetTranslationsForWordsByLanguageQuery query,
                                                                          CancellationToken cancellationToken = default(CancellationToken))
        {
            var words = await _database.Word
                                       .Include(w => w.Translation)
                                       .Where(w => query.Words.Contains(w.Title)).ToListAsync(cancellationToken);

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
    }
}