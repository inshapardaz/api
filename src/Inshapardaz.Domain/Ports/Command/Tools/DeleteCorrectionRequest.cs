using Inshapardaz.Domain.Adapters.Repositories;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Tools;

public class DeleteCorrectionRequest(string language, string profile, long correctionId) : RequestBase
{
    public string Language { get; } = language;
    public string Profile { get; } = profile;
    public long CorrectionId { get; } = correctionId;
}

public class DeleteCorrectionRequestHandler(ICorrectionRepository correctionRepository)
    : RequestHandlerAsync<DeleteCorrectionRequest>
{
    [AuthorizeAdmin(1)]
    public override async Task<DeleteCorrectionRequest> HandleAsync(DeleteCorrectionRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var author = await correctionRepository.GetCorrection(command.Language, command.Profile, command.CorrectionId, cancellationToken);
        if (author != null)
        {
            await correctionRepository.DeleteCorrection(command.CorrectionId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
