using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Tools.CommonWords;

public class GetCommonWordsQuery : IQuery<Page<CommonWordModel>>
{
    public GetCommonWordsQuery(string language)
    {
        Language = language;
    }
    public string Language { get; init; }
    public string Query { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class GetCommonWordsQueryHandler : QueryHandlerAsync<GetCommonWordsQuery, Page<CommonWordModel>>
{
    private readonly ICommonWordsRepository _correctionRepository;

    public GetCommonWordsQueryHandler(ICommonWordsRepository correctionRepository)
    {
        _correctionRepository = correctionRepository;
    }

    public override async Task<Page<CommonWordModel>> ExecuteAsync(GetCommonWordsQuery query, CancellationToken cancellationToken = default)
        => await _correctionRepository.GetWords(query.Language, query.Query, query.PageNumber, query.PageSize, cancellationToken);

}
