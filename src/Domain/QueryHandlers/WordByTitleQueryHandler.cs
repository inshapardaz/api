using Darker;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Model;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class WordByTitleQueryHandler : AsyncQueryHandler<WordByTitleQuery, Word>
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