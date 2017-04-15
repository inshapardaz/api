using System.Linq;
using Darker;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class WordQueryHandler : QueryHandler<WordQuery, WordQuery.Response>
    {
        private readonly IDatabaseContext _database;

        public WordQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override WordQuery.Response Execute(WordQuery query)
        {
            var words = _database.Words;
            var count = words.Count();

            var data = words.OrderBy(x => x.Title)
                            .Paginate(query.PageNumber, query.PageSize)
                            .ToList();

            return new WordQuery.Response
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
