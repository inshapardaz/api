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
    public class GetWordsByIdsQueryHandler : QueryHandlerAsync<GetWordsByIdsQuery, Page<Word>>
    {
        private readonly IDatabaseContext _database;

        public GetWordsByIdsQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<Page<Word>> ExecuteAsync(GetWordsByIdsQuery query, CancellationToken cancellationToken)
        {
            var wordIndices = query.DictionaryId > 0
                ? _database.Word.Where(
                    x => x.DictionaryId == query.DictionaryId && query.IDs.Contains(x.Id))
                : _database.Word.Where(x => query.IDs.Contains(x.Id));

            var count = query.IDs.Count();
            var data = await wordIndices
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