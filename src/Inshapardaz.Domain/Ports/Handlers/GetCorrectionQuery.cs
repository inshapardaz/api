using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Paramore.Darker;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers
{
    public class GetCorrectionQuery : IQuery<CorrectionModel>
    {
        public string Language { get; set; }
        public string Profile { get; set; }
        public string IncorrectText { get; set; }
    }

    public class GetCorrectionQueryHandler : QueryHandlerAsync<GetCorrectionQuery, CorrectionModel>
    {
        private readonly ICorrectionRepository _correctionRepository;

        public GetCorrectionQueryHandler(ICorrectionRepository correctionRepository)
        {
            _correctionRepository = correctionRepository;
        }

        public async override Task<CorrectionModel> ExecuteAsync(GetCorrectionQuery query, CancellationToken cancellationToken = default)
            => await _correctionRepository.GetCorrection(query.Language, query.Profile, query.IncorrectText, cancellationToken);

    }
}
