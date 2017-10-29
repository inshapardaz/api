using System.Linq;
using Paramore.Darker;
using Inshapardaz.Domain.Queries;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetTranslationsByWordIdQueryHandler : QueryHandlerAsync<GetTranslationsByWordIdQuery,
        IEnumerable<Translation>>
    {
        private readonly IDatabaseContext _database;

        public GetTranslationsByWordIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<IEnumerable<Translation>> ExecuteAsync(GetTranslationsByWordIdQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _database.Translation
                .Where(t => t.WordId == query.WordId)
                .ToListAsync(cancellationToken);
        }
    }
}