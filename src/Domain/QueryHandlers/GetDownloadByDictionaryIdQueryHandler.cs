﻿using System.Linq;
using Inshapardaz.Domain.Queries;
using Darker;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDownloadByDictionaryIdQueryHandler : AsyncQueryHandler<GetDownloadByDictionaryIdQuery, File>
    {
        private readonly IDatabaseContext _database;

        public GetDownloadByDictionaryIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<File> ExecuteAsync(GetDownloadByDictionaryIdQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<Dictionary> result;
            if (!string.IsNullOrWhiteSpace(query.UserId))
            {
                result = _database.Dictionary
                                  .Include(d => d.Downloads)
                                  .ThenInclude(d => d.File)
                                  .Where(d => d.Id == query.DictionaryId && 
                                              (d.IsPublic || d.UserId == query.UserId));
            }
            else
            {
                result = _database.Dictionary
                                  .Include(d => d.Downloads)
                                  .ThenInclude(d => d.File)
                                  .Where(d => d.Id == query.DictionaryId && d.IsPublic);
            }

            Dictionary dictionary = await result.SingleOrDefaultAsync(cancellationToken);
            return dictionary?.Downloads?.SingleOrDefault(d => d.MimeType == query.MimeType)?.File;
        }
    }
}