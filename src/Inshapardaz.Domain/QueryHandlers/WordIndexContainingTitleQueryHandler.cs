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
    public class WordIndexContainingTitleQueryHandler : QueryHandlerAsync<WordContainingTitleQuery, Page<Word>>
    {
        private readonly IDatabaseContext _database;

        public WordIndexContainingTitleQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<Page<Word>> ExecuteAsync(WordContainingTitleQuery query, CancellationToken cancellationToken)
        {
            var wordIndices = query.DictionaryId > 0
                ? _database.Word.Where(
                    x => x.DictionaryId == query.DictionaryId && x.Title.StartsWith(query.SearchTerm))
                : _database.Word.Where(x => x.Title.StartsWith(query.SearchTerm));

            var count = await wordIndices.CountAsync(cancellationToken);
            var data = wordIndices
                .OrderBy(x => x.Title.Length)
                .ThenBy(x => x.Title)
                .Paginate(query.PageNumber, query.PageSize)
                .ToList();

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