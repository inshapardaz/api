using System.Linq;
using Darker;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Helpers;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class WordStartingWithQueryHandler : QueryHandler<WordStartingWithQuery, Page<Word>>
    {
        private readonly IDatabaseContext _database;

        public WordStartingWithQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override Page<Word> Execute(WordStartingWithQuery request)
        {
            var wordIndices = _database.Word.Where(x => x.DictionaryId == request.DictionaryId &&
                                                        x.Title.StartsWith(request.Title));

            var count = wordIndices.Count();
            var data = wordIndices
                .OrderBy(x => x.Title.Length)
                .ThenBy(x => x.Title)
                .Paginate(request.PageNumber, request.PageSize)
                .ToList();

            return new Page<Word>
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = count,
                Data = data
            };
        }
    }
}