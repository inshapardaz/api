using System;
using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Domain.Queries;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionariesQueryHandler : QueryHandlerAsync<GetDictionariesQuery,
        IEnumerable<Dictionary>>
    {
        private readonly IDatabaseContext _database;

        public GetDictionariesQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<IEnumerable<Dictionary>> ExecuteAsync(GetDictionariesQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _database.Dictionary.ToListAsync(cancellationToken);
        }
    }
}