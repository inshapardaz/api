using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Ports.Command;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class AddCorrectionRequest : RequestBase
    {
        public AddCorrectionRequest(CorrectionModel correctionModel)
        {
            Correction = correctionModel;
        }

        public CorrectionModel Correction { get; }
        public CorrectionModel Result { get; set; }
    }

    public class AddCorrectionRequestHandler : RequestHandlerAsync<AddCorrectionRequest>
    {
        private readonly ICorrectionRepository _correctionRepository;

        public AddCorrectionRequestHandler(ICorrectionRepository correctionRepository)
        {
            _correctionRepository = correctionRepository;
        }

        [AuthorizeAdmin(1)]
        public override async Task<AddCorrectionRequest> HandleAsync(AddCorrectionRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result =  await _correctionRepository.AddCorrection(command.Correction, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
