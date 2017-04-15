using System.Linq;

using Darker;

using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class WordIndexContainingTitleQueryHandler : QueryHandler<WordContainingTitleQuery, WordContainingTitleQuery.Response>
    {
        private readonly IDatabaseContext _database;

        public WordIndexContainingTitleQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override WordContainingTitleQuery.Response Execute(WordContainingTitleQuery query)
        {
            var wordIndices = _database.Words.Where(x => x.Title.StartsWith(query.SearchTerm));

            var count = wordIndices.Count();
            var data = wordIndices
                            .OrderBy(x => x.Title.Length)
                            .ThenBy(x => x.Title)
                            .Paginate(query.PageNumber, query.PageSize)
                            .ToList();

            return new WordContainingTitleQuery.Response
            {
                Page = new Page<Word>
                {
                    PageNumber = query.PageNumber,
                    PageSize = query.PageSize,
                    TotalCount = count,
                    Data = data
                }
            };
        }
    }
}
