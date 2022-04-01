using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers
{
    public class GetAutoFixListCommand : RequestBase
    {
        public string Language { get; set; }
        public Dictionary<string, string> Result { get; internal set; }
    }

    public class GetAutoFixListCommandHandeler : RequestHandlerAsync<GetAutoFixListCommand>
    {
        private readonly ICorrectionRepository _correctionRepository;

        public GetAutoFixListCommandHandeler(ICorrectionRepository correctionRepository)
        {
            _correctionRepository = correctionRepository;
        }

        public override async Task<GetAutoFixListCommand> HandleAsync(GetAutoFixListCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _correctionRepository.GetAutoCorrectionList(command.Language, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
