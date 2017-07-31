using System.Linq;
using Darker;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class WordRelationsByTitleQueryHandler : QueryHandler<WordRelationsByTitleQuery, Page<Word>>
    {
        private readonly IDatabaseContext _database;

        public WordRelationsByTitleQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override Page<Word> Execute(WordRelationsByTitleQuery request)
        {
            var relations = _database.Word
                            .Where(x => x.Title == request.Title)
                            .SelectMany(w => w.WordRelationRelatedWord)
                            .Select(x => x.RelatedWord);

            var count = relations.Count();
            var data = relations
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