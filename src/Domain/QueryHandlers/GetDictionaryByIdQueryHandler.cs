using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Domain.Queries;
using Darker;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionaryByIdQueryHandler : AsyncQueryHandler<DictionaryByIdQuery, Dictionary>
    {
        private readonly IDatabaseContext _database;

        public GetDictionaryByIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<Dictionary> ExecuteAsync(DictionaryByIdQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<Dictionary> result;
            if (!string.IsNullOrWhiteSpace(query.UserId))
            {
                result = _database.Dictionary.Where(d => d.Id == query.DictionaryId &&
                                                         (d.IsPublic || (d.UserId == query.UserId)));
            }
            else
            {
                result = _database.Dictionary.Where(d => d.Id == query.DictionaryId && d.IsPublic);
            }

            return await result.SingleOrDefaultAsync(cancellationToken);
        }
    }
}