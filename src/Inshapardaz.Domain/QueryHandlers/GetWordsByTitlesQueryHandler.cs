using Paramore.Darker;
using Inshapardaz.Domain.Queries;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordsByTitlesQueryHandler : QueryHandlerAsync<GetWordsByTitlesQuery, IEnumerable<Word>>
    {
        private readonly IDatabaseContext _database;

        public GetWordsByTitlesQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<IEnumerable<Word>> ExecuteAsync(GetWordsByTitlesQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _database.Word
                        .Where(x => x.DictionaryId == query.DictionaryId && query.Titles.Contains(x.Title))
                        .ToListAsync(cancellationToken);
        }
    }
}