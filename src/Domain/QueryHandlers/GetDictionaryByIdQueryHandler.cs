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
    public class GetDictionaryByIdQueryHandler : AsyncQueryHandler<DictionaryByIdQuery, Dictionary>
    {
        private readonly IDatabaseContext _database;

        public GetDictionaryByIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public async override Task<Dictionary> ExecuteAsync(DictionaryByIdQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<Dictionary> result;
            if (!string.IsNullOrWhiteSpace(query.UserId))
            {
                result = _database.Dictionaries.Where(d => d.Id == query.DictionaryId && (d.IsPublic || (d.UserId == query.UserId)));
            }
            else
            {
                result = _database.Dictionaries.Where(d => d.Id == query.DictionaryId && d.IsPublic);
            }

            return await result.SingleOrDefaultAsync();
        }
    }
}
