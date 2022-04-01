using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers
{
    public class GetPunctuationListCommand : RequestBase
    {
        public string Language { get; set; }
        public Dictionary<string, string> Result { get; internal set; }
    }

    public class GetPunctuationListCommandHandeler : RequestHandlerAsync<GetPunctuationListCommand>
    {
        private readonly ICorrectionRepository _correctionRepository;

        public GetPunctuationListCommandHandeler(ICorrectionRepository correctionRepository)
        {
            _correctionRepository = correctionRepository;
        }

        public override async Task<GetPunctuationListCommand> HandleAsync(GetPunctuationListCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _correctionRepository.GetPunctuationList(command.Language, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
