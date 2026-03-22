using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Tools;

public class GetCorrectionQuery : IQuery<CorrectionModel>
{
    public long Id { get; set; }
    public string Language { get; set; }
    public string Profile { get; set; }
}

public class GetCorrectionQueryHandler(ICorrectionRepository correctionRepository)
    : QueryHandlerAsync<GetCorrectionQuery, CorrectionModel>
{
    [AuthorizeAdmin(1)]
    public async override Task<CorrectionModel> ExecuteAsync(GetCorrectionQuery query, CancellationToken cancellationToken = default)
        => await correctionRepository.GetCorrection(query.Language, query.Profile, query.Id, cancellationToken);

}
