using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Tools.CommonWords;

public class GetCommonWordsQuery(string language) : IQuery<Page<CommonWordModel>>
{
    public string Language { get; init; } = language;
    public string Query { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class GetCommonWordsQueryHandler(ICommonWordsRepository correctionRepository)
    : QueryHandlerAsync<GetCommonWordsQuery, Page<CommonWordModel>>
{
    public override async Task<Page<CommonWordModel>> ExecuteAsync(GetCommonWordsQuery query, CancellationToken cancellationToken = default)
        => await correctionRepository.GetWords(query.Language, query.Query, query.PageNumber, query.PageSize, cancellationToken);

}
