using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;
using Paramore.Darker;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers
{
    public class GetCorrectionsQuery : IQuery<Page<CorrectionModel>>
    {
        public string Language { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Query { get; set; }
        public string Profile { get; set; }
    }

    public class GetCorrectionsQueryHandler : QueryHandlerAsync<GetCorrectionsQuery, Page<CorrectionModel>>
    {
        private readonly ICorrectionRepository _correctionRepository;

        public GetCorrectionsQueryHandler(ICorrectionRepository correctionRepository)
        {
            _correctionRepository = correctionRepository;
        }

        public async override Task<Page<CorrectionModel>> ExecuteAsync(GetCorrectionsQuery query, CancellationToken cancellationToken = default)
        => await _correctionRepository.GetCorrectionList(query.Language, query.Query, query.Profile, query.PageNumber, query.PageSize, cancellationToken);
    }
}
