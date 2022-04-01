using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers
{
    public class GetCorrectionsListCommand : RequestBase
    {
        public string Language { get; set; }
        public Dictionary<string, string> Result { get; internal set; }
    }

    public class GetCorrectionsListCommandHandeler : RequestHandlerAsync<GetCorrectionsListCommand>
    {
        private readonly ICorrectionRepository _correctionRepository;

        public GetCorrectionsListCommandHandeler(ICorrectionRepository correctionRepository)
        {
            _correctionRepository = correctionRepository;
        }

        public override async Task<GetCorrectionsListCommand> HandleAsync(GetCorrectionsListCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _correctionRepository.GetCorrectionList(command.Language, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
