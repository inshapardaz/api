using Paramore.Darker;
using Inshapardaz.Domain.Queries;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class WordByTitleQueryHandler : QueryHandlerAsync<WordByTitleQuery, Word>
    {
        private readonly IDatabaseContext _database;

        public WordByTitleQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<Word> ExecuteAsync(WordByTitleQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _database.Word.SingleOrDefaultAsync(x => x.Title == query.Title, cancellationToken);
        }
    }
}