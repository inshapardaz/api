using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Ports.Command;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class DeleteCorrectionRequest : RequestBase
    {
        public DeleteCorrectionRequest(string language, string profile, long id)
        {
            Language = language;
            Profile = profile;
            Id = id;
        }

        public string Language { get; }
        public string Profile { get; }
        public long Id { get; }
    }

    public class DeleteCorrectionRequestHandler : RequestHandlerAsync<DeleteCorrectionRequest>
    {
        private readonly ICorrectionRepository _corretionRepository;

        public DeleteCorrectionRequestHandler(ICorrectionRepository correctionRepository)
        {
            _corretionRepository = correctionRepository;
        }

        [AuthorizeAdmin(1)]
        public override async Task<DeleteCorrectionRequest> HandleAsync(DeleteCorrectionRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var author = await _corretionRepository.GetCorrection(command.Language, command.Profile, command.Id, cancellationToken);
            if (author != null)
            {
                await _corretionRepository.DeleteCorrection(command.Id, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
