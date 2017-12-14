using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Queries;
using Microsoft.EntityFrameworkCore;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionaryDownloadsQueryHandler : QueryHandlerAsync<GetDictionaryDownloadsQuery, IEnumerable<DictionaryDownload>>
    {
        private readonly IDatabaseContext _database;

        public GetDictionaryDownloadsQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<IEnumerable<DictionaryDownload>> ExecuteAsync(GetDictionaryDownloadsQuery query,
                                                      CancellationToken cancellationToken = default(CancellationToken))
        {         
            if (query.UserId == Guid.Empty)
            {
                return await _database.DictionaryDownload
                                  .Where(d => d.DictionaryId == query.DictionaryId && d.Dictionary.IsPublic)
                                  .ToListAsync(cancellationToken);
                
            }

            return await _database.DictionaryDownload
                                  .Where(d => d.DictionaryId == query.DictionaryId && (d.Dictionary.IsPublic || d.Dictionary.UserId == query.UserId))
                                  .ToListAsync(cancellationToken);

        }
    }
}