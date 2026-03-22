using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Tools;

public class GetAllCorrectionsQuery : IQuery<IEnumerable<CorrectionModel>>
{
    public string Language { get; set; }
    public string Profile { get; set; }
}

public class GetAllCorrectionsQueryHandler(ICorrectionRepository correctionRepository)
    : QueryHandlerAsync<GetAllCorrectionsQuery, IEnumerable<CorrectionModel>>
{
    public async override Task<IEnumerable<CorrectionModel>> ExecuteAsync(GetAllCorrectionsQuery query, CancellationToken cancellationToken = default)
        => await correctionRepository.GetAllCorrections(query.Language, query.Profile, cancellationToken);
}
