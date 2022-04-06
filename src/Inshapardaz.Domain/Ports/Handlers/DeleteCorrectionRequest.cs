using Inshapardaz.Domain.Adapters.Repositories;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class DeleteCorrectionRequest : RequestBase
    {
        public DeleteCorrectionRequest(string language, string profile, string incorrectText)
        {
            Language = language;
            Profile = profile;
            IncorrectText = incorrectText;
        }

        public string Language { get; }
        public string Profile { get; }
        public string IncorrectText { get; }
    }

    public class DeleteCorrectionRequestHandler : RequestHandlerAsync<DeleteCorrectionRequest>
    {
        private readonly ICorrectionRepository _corretionRepository;

        public DeleteCorrectionRequestHandler(ICorrectionRepository correctionRepository)
        {
            _corretionRepository = correctionRepository;
        }

        public override async Task<DeleteCorrectionRequest> HandleAsync(DeleteCorrectionRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var author = await _corretionRepository.GetCorrection(command.Language, command.Profile, command.IncorrectText, cancellationToken);
            if (author != null)
            {
                await _corretionRepository.DeleteCorrection(command.Language, command.Profile, command.IncorrectText, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
