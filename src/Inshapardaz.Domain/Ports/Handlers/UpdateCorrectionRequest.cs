using Inshapardaz.Domain.Adapters.Repositories;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class UpdateCorrectionRequest : RequestBase
    {
        public UpdateCorrectionRequest(CorrectionModel correctionModel)
        {
            Correction = correctionModel;
        }

        public CorrectionModel Correction { get; }
        public RequestResult Result { get; set; } = new RequestResult();
        public class RequestResult
        {
            public CorrectionModel Correction { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateCorrectionRequestHandler : RequestHandlerAsync<UpdateCorrectionRequest>
    {
        private readonly ICorrectionRepository _correctionRepository;

        public UpdateCorrectionRequestHandler(ICorrectionRepository correctionRepository)
        {
            _correctionRepository = correctionRepository;
        }

        public override async Task<UpdateCorrectionRequest> HandleAsync(UpdateCorrectionRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _correctionRepository.GetCorrection(command.Correction.Language, command.Correction.Profile, command.Correction.Id, cancellationToken);

            if (result == null)
            {   
                command.Result.Correction =  await _correctionRepository.AddCorrection(command.Correction, cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                command.Result.Correction = await _correctionRepository.UpdateCorrection(command.Correction, cancellationToken); ;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
