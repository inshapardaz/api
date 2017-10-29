using Paramore.Darker;
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
    public class GetTranslationsByLanguageQueryHandler : QueryHandlerAsync<GetTranslationsByLanguageQuery,
        IEnumerable<Translation>>
    {
        private readonly IDatabaseContext _database;

        public GetTranslationsByLanguageQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<IEnumerable<Translation>> ExecuteAsync(GetTranslationsByLanguageQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _database.Translation
                .Where(t => t.WordId == query.WordId && t.Language == query.Language)
                .ToListAsync(cancellationToken);
        }
    }
}