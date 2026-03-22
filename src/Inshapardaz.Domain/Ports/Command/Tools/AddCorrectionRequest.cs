using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Tools;

public class AddCorrectionRequest(CorrectionModel correctionModel) : RequestBase
{
    public CorrectionModel Correction { get; } = correctionModel;
    public CorrectionModel Result { get; set; }
}

public class AddCorrectionRequestHandler(ICorrectionRepository correctionRepository)
    : RequestHandlerAsync<AddCorrectionRequest>
{
    [AuthorizeAdmin(1)]
    public override async Task<AddCorrectionRequest> HandleAsync(AddCorrectionRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        command.Result = await correctionRepository.AddCorrection(command.Correction, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
