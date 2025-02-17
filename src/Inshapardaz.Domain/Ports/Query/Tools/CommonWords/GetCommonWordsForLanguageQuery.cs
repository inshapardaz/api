using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Adapters.Repositories;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Tools.CommonWords;

public class GetAllWordsForLanguageQuery : IQuery<IEnumerable<string>>
{
    public GetAllWordsForLanguageQuery(string language)
    {
        Language = language;
    }
    public string Language { get; init; }
}

public class GetAllWordsForLanguageQueryHandler : QueryHandlerAsync<GetAllWordsForLanguageQuery, IEnumerable<string>>
{
    private readonly ICommonWordsRepository _correctionRepository;

    public GetAllWordsForLanguageQueryHandler(ICommonWordsRepository correctionRepository)
    {
        _correctionRepository = correctionRepository;
    }
    
    public override async Task<IEnumerable<string>> ExecuteAsync(GetAllWordsForLanguageQuery query, CancellationToken cancellationToken = default)
        => await _correctionRepository.GetWordsForLanguage(query.Language, cancellationToken);

}
