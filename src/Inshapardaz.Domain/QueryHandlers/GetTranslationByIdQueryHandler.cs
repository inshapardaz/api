using Paramore.Darker;
using Inshapardaz.Domain.Queries;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetTranslationByIdQueryHandler : QueryHandlerAsync<GetTranslationByIdQuery, Translation>
    {
        private readonly IDatabaseContext _database;

        public GetTranslationByIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<Translation> ExecuteAsync(GetTranslationByIdQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _database.Translation.SingleOrDefaultAsync(t => t.Id == query.Id, cancellationToken);
        }
    }
}