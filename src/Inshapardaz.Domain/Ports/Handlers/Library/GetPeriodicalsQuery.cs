using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class GetPeriodicalsQuery : LibraryBaseQuery<Page<PeriodicalModel>>
    {
        public GetPeriodicalsQuery(int libraryId, int pageNumber, int pageSize)
            : base(libraryId)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public string Query { get; set; }
    }

    public class GetPeriodicalsQueryHandler : QueryHandlerAsync<GetPeriodicalsQuery, Page<PeriodicalModel>>
    {
        private readonly IPeriodicalRepository _periodicalRepository;

        public GetPeriodicalsQueryHandler(IPeriodicalRepository periodicalRepository)
        {
            _periodicalRepository = periodicalRepository;
        }

        public override async Task<Page<PeriodicalModel>> ExecuteAsync(GetPeriodicalsQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (string.IsNullOrWhiteSpace(command.Query))
            {
                return await _periodicalRepository.GetPeriodicals(command.LibraryId, command.PageNumber, command.PageSize, cancellationToken);
            }

            return await _periodicalRepository.SearchPeriodicals(command.LibraryId, command.Query, command.PageNumber, command.PageSize, cancellationToken);
        }
    }
}
