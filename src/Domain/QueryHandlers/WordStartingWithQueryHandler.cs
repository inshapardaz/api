using System.Linq;

using Darker;

using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Helpers;
using System;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class WordStartingWithQueryHandler : QueryHandler<WordStartingWithQuery, WordStartingWithQuery.Response>
    {
        private readonly IDatabaseContext _database;

        public WordStartingWithQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override WordStartingWithQuery.Response Execute(WordStartingWithQuery request)
        {
            var wordIndices = _database.Words.Where(x => x.Title.StartsWith(request.Title));

            var count = wordIndices.Count();
            var data = wordIndices
                            .OrderBy(x => x.Title.Length)
                            .ThenBy(x => x.Title)
                            .Paginate(request.PageNumber, request.PageSize)
                            .ToList();

            return new WordStartingWithQuery.Response()
            {
                Page = new Page<Word>
                {
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = count,
                    Data = data
                }
            };
        }
    }
}
