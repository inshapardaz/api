using Inshapardaz.Domain.Adapters.Repositories;
using Paramore.Darker;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers
{
    public class GetAllCorrectionsQuery : IQuery<Dictionary<string, string>>
    {
        public string Language { get; set; }
        public string Profile { get; set; }
    }

    public class GetAllCorrectionsQueryHandler : QueryHandlerAsync<GetAllCorrectionsQuery, Dictionary<string, string>>
    {
        private readonly ICorrectionRepository _correctionRepository;

        public GetAllCorrectionsQueryHandler(ICorrectionRepository correctionRepository)
        {
            _correctionRepository = correctionRepository;
        }

        public async override Task<Dictionary<string, string>> ExecuteAsync(GetAllCorrectionsQuery query, CancellationToken cancellationToken = default)
            => await _correctionRepository.GetAllCorrections(query.Language, query.Profile, cancellationToken);
    }
}
