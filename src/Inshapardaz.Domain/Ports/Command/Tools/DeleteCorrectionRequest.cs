using Inshapardaz.Domain.Adapters.Repositories;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Tools;

public class DeleteCorrectionRequest : RequestBase
{
    public DeleteCorrectionRequest(string language, string profile, long correctionId)
    {
        Language = language;
        Profile = profile;
        CorrectionId = correctionId;
    }

    public string Language { get; }
    public string Profile { get; }
    public long CorrectionId { get; }
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
        var author = await _corretionRepository.GetCorrection(command.Language, command.Profile, command.CorrectionId, cancellationToken);
        if (author != null)
        {
            await _corretionRepository.DeleteCorrection(command.CorrectionId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
