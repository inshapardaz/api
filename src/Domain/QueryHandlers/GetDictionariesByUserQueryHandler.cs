using System.Collections.Generic;
using System.Linq;

using Inshapardaz.Domain.Queries;

using Darker;
using Inshapardaz.Domain.Model;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionariesByUserQueryHandler : AsyncQueryHandler<DictionariesByUserQuery, IEnumerable<Model.Dictionary>>
    {
        private readonly IDatabaseContext _database;

        public GetDictionariesByUserQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public async override Task<IEnumerable<Dictionary>> ExecuteAsync(DictionariesByUserQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<Dictionary> result;
            if (!string.IsNullOrWhiteSpace(query.UserId))
            {
                result = _database.Dictionaries.Where(d => d.IsPublic || (d.UserId == query.UserId));
            }
            else
            {
                result = _database.Dictionaries.Where(d => d.IsPublic);
            }

            return await result.ToListAsync();
        }
    }
}
