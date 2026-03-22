using Inshapardaz.Domain.Adapters.Repositories;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Tools.CommonWords;

public class GetAllWordsForLanguageQuery(string language) : IQuery<IEnumerable<string>>
{
    public string Language { get; init; } = language;
}

public class GetAllWordsForLanguageQueryHandler(ICommonWordsRepository correctionRepository)
    : QueryHandlerAsync<GetAllWordsForLanguageQuery, IEnumerable<string>>
{
    public override async Task<IEnumerable<string>> ExecuteAsync(GetAllWordsForLanguageQuery query, CancellationToken cancellationToken = default)
        => await correctionRepository.GetWordsForLanguage(query.Language, cancellationToken);

}
