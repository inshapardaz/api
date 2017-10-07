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
    public class TranslationsByWordDetailAndLanguageQueryHandler : AsyncQueryHandler<TranslationsByWordDetailAndLanguageQuery,
        IEnumerable<Translation>>
    {
        private readonly IDatabaseContext _database;

        public TranslationsByWordDetailAndLanguageQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<IEnumerable<Translation>> ExecuteAsync(TranslationsByWordDetailAndLanguageQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _database.Translation
                .Where(t => t.WordDetail.Id == query.WordDetailId && t.Language == query.Language)
                .ToListAsync(cancellationToken);
        }
    }
}