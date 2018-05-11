using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordRelationsByTitleQueryHandler : QueryHandlerAsync<GetWordRelationsByTitleQuery, Page<Word>>
    {
        public override async Task<Page<Word>> ExecuteAsync(GetWordRelationsByTitleQuery request, CancellationToken cancellationToken)
        {
            //var relations = _database.Word
            //                .Where(x => x.Title == request.Title)
            //                .SelectMany(w => w.Relations)
            //                .Where(r => r.RelationType == RelationType.Synonym)
            //                .Select(x => x.SourceWord);

            //var count = await relations.CountAsync(cancellationToken);
            //var data = relations
            //                .OrderBy(x => x.Title.Length)
            //                .ThenBy(x => x.Title)
            //                .Paginate(request.PageNumber, request.PageSize)
            //                .ToList();

            //return new Page<Word>
            //{
            //    PageNumber = request.PageNumber,
            //    PageSize = request.PageSize,
            //    TotalCount = count,
            //    Data = data
            //};
            throw new NotImplementedException();
        }
    }
}