using Paramore.Darker;
using Inshapardaz.Domain.Queries;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordByIdQueryHandler : QueryHandlerAsync<GetWordByIdQuery, Word>
    {
        private readonly IDatabaseContext _database;

        public GetWordByIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<Word> ExecuteAsync(GetWordByIdQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _database.Word.SingleOrDefaultAsync(w => w.Id == query.WordId, cancellationToken);
        }
    }
}