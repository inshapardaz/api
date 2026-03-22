using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Tools;

public class GetCorrectionsQuery : IQuery<Page<CorrectionModel>>
{
    public string Language { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string Query { get; set; }
    public string Profile { get; set; }
}

public class GetCorrectionsQueryHandler(ICorrectionRepository correctionRepository)
    : QueryHandlerAsync<GetCorrectionsQuery, Page<CorrectionModel>>
{
    public async override Task<Page<CorrectionModel>> ExecuteAsync(GetCorrectionsQuery query, CancellationToken cancellationToken = default)
    => await correctionRepository.GetCorrectionList(query.Language, query.Query, query.Profile, query.PageNumber, query.PageSize, cancellationToken);
}
