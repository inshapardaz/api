using System.Linq;
using Darker;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Queries;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class WordIndexContainingTitleQueryHandler : QueryHandler<WordContainingTitleQuery, Page<Word>>
    {
        private readonly IDatabaseContext _database;

        public WordIndexContainingTitleQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override Page<Word> Execute(WordContainingTitleQuery query)
        {
            var wordIndices = query.DictionaryId > 0
                ? _database.Word.Where(
                    x => x.DictionaryId == query.DictionaryId && x.Title.StartsWith(query.SearchTerm))
                : _database.Word.Where(x => x.Title.StartsWith(query.SearchTerm));

            var count = wordIndices.Count();
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