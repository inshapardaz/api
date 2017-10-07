using Darker;
using Inshapardaz.Domain.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class TranslationsByLanguageQueryHandler : AsyncQueryHandler<TranslationsByLanguageQuery,
        IEnumerable<Translation>>
    {
        private readonly IDatabaseContext _database;

        public TranslationsByLanguageQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<IEnumerable<Translation>> ExecuteAsync(TranslationsByLanguageQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _database.Translation
                .Where(t => t.WordDetail.WordInstanceId == query.WordId && t.Language == query.Language)
                .ToListAsync(cancellationToken);
        }
    }
}