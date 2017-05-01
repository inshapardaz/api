using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Darker;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordsPagesQueryHandler : AsyncQueryHandler<WordQuery, Page<Word>>
    {
        private readonly IDatabaseContext _database;

        public GetWordsPagesQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }
        
        public async override Task<Page<Word>> ExecuteAsync(WordQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            var words = _database.Words;
            var count = words.Count();

            var data = await words.OrderBy(x => x.Title)
                            .Paginate(query.PageNumber, query.PageSize)
                            .ToListAsync();

            return new Page<Word>
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = count,
                Data = data
            };
        }
    }
}
