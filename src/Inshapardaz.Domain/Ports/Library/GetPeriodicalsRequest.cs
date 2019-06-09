using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetPeriodicalsRequest : RequestBase
    {
        public GetPeriodicalsRequest(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public Page<Periodical> Result { get; set; }

        public string Query { get; set; }
    }

    public class GetPeriodicalsRequestHandler : RequestHandlerAsync<GetPeriodicalsRequest>
    {
        private readonly IPeriodicalRepository _periodicalRepository;

        public GetPeriodicalsRequestHandler(IPeriodicalRepository periodicalRepository)
        {
            _periodicalRepository = periodicalRepository;
        }

        public override async Task<GetPeriodicalsRequest> HandleAsync(GetPeriodicalsRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (string.IsNullOrWhiteSpace(command.Query))
            {
                command.Result = await _periodicalRepository.GetPeriodicals(command.PageNumber, command.PageSize, cancellationToken);
            }
            else
            {
                command.Result = await _periodicalRepository.SearchPeriodicals(command.Query, command.PageNumber, command.PageSize, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
