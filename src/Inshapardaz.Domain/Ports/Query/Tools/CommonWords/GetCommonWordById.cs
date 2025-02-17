using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Tools.CommonWords;

public class GetCommonWordByIdQuery : IQuery<CommonWordModel>
{
    public GetCommonWordByIdQuery(string language, long id)
    {
        Language = language;
        Id = id;
    }

    public string Language { get; }
    public long Id { get; init; }
}

public class GetCommonWordsForLanguageQueryHandler : QueryHandlerAsync<GetCommonWordByIdQuery, CommonWordModel>
{
    private readonly ICommonWordsRepository _correctionRepository;

    public GetCommonWordsForLanguageQueryHandler(ICommonWordsRepository correctionRepository)
    {
        _correctionRepository = correctionRepository;
    }
    
    public override async Task<CommonWordModel> ExecuteAsync(GetCommonWordByIdQuery query, CancellationToken cancellationToken = default)
        => await _correctionRepository.GetWordById(query.Language, query.Id, cancellationToken);

}
