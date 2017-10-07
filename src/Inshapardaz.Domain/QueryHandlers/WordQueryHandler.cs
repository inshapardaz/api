using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Paramore.Darker;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordsPagesQueryHandler : QueryHandlerAsync<GetWordPageQuery, Page<Word>>
    {
        private readonly IDatabaseContext _database;

        public GetWordsPagesQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<Page<Word>> ExecuteAsync(GetWordPageQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var words = _database.Word.Where(w => w.DictionaryId == query.DictionaryId);
            var count = words.Count();

            var data = await words
                .OrderBy(x => x.Title)
                .Paginate(query.PageNumber, query.PageSize)
                .ToListAsync(cancellationToken);

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