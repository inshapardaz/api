using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Paramore.Darker;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetWordStartingWithQueryHandler : QueryHandlerAsync<GetWordStartingWithQuery, Page<Word>>
    {
        private readonly IDatabaseContext _database;

        public GetWordStartingWithQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<Page<Word>> ExecuteAsync(GetWordStartingWithQuery request, CancellationToken cancellationToken)
        {
            var wordIndices = _database.Word.Where(x => x.DictionaryId == request.DictionaryId &&
                                                        x.Title.StartsWith(request.Title));

            var count = await wordIndices.CountAsync(cancellationToken);
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