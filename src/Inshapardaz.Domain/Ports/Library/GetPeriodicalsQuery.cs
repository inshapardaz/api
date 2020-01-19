using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetPeriodicalsQuery : IQuery<Page<Periodical>>
    {
        public GetPeriodicalsQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public string Query { get; set; }
    }

    public class GetPeriodicalsQueryHandler : QueryHandlerAsync<GetPeriodicalsQuery, Page<Periodical>>
    {
        private readonly IPeriodicalRepository _periodicalRepository;

        public GetPeriodicalsQueryHandler(IPeriodicalRepository periodicalRepository)
        {
            _periodicalRepository = periodicalRepository;
        }

        public override async Task<Page<Periodical>> ExecuteAsync(GetPeriodicalsQuery command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (string.IsNullOrWhiteSpace(command.Query))
            {
                return await _periodicalRepository.GetPeriodicals(command.PageNumber, command.PageSize, cancellationToken);
            }
            
            return await _periodicalRepository.SearchPeriodicals(command.Query, command.PageNumber, command.PageSize, cancellationToken);
        }
    }
}
