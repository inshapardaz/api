using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class UpdatePeriodicalRequest : RequestBase
    {
        public UpdatePeriodicalRequest(Periodical periodical)
        {
            Periodical = periodical;
        }

        public Periodical Periodical { get; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public Periodical Periodical { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdatePeriodicalRequestHandler : RequestHandlerAsync<UpdatePeriodicalRequest>
    {
        private readonly IPeriodicalRepository _periodicalRepository;

        public UpdatePeriodicalRequestHandler(IPeriodicalRepository periodicalRepository)
        {
            _periodicalRepository = periodicalRepository;
        }

        public override async Task<UpdatePeriodicalRequest> HandleAsync(UpdatePeriodicalRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _periodicalRepository.GetPeriodicalById(command.Periodical.Id, cancellationToken);

            if (result == null)
            {
                var periodical = command.Periodical;
                periodical.Id = default(int);
                command.Result.Periodical =  await _periodicalRepository.AddPeriodical(periodical, cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                await _periodicalRepository.UpdatePeriodical(command.Periodical, cancellationToken);
                command.Result.Periodical = command.Periodical;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}