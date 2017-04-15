using System.Linq;
using Darker;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class WordRelationsByTitleQueryHandler : QueryHandler<WordRelationsByTitleQuery, WordRelationsByTitleQuery.Response>
    {
        private readonly IDatabase _database;

        public WordRelationsByTitleQueryHandler(IDatabase database)
        {
            _database = database;
        }

        public override WordRelationsByTitleQuery.Response Execute(WordRelationsByTitleQuery request)
        {
            var relations = _database.Words
                            .Where(x => x.Title == request.Title)
                            .SelectMany(w => w.WordRelations)
                            .Select(x => x.RelatedWord);

            var count = relations.Count();
            var data = relations
                            .OrderBy(x => x.Title.Length)
                            .ThenBy(x => x.Title)
                            .Paginate(request.PageNumber, request.PageSize)
                            .ToList();

            return new WordRelationsByTitleQuery.Response()
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
