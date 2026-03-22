using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Tools.CommonWords;

public class GetCommonWordByIdQuery(string language, long id) : IQuery<CommonWordModel>
{
    public string Language { get; } = language;
    public long Id { get; init; } = id;
}

public class GetCommonWordsForLanguageQueryHandler(ICommonWordsRepository correctionRepository)
    : QueryHandlerAsync<GetCommonWordByIdQuery, CommonWordModel>
{
    public override async Task<CommonWordModel> ExecuteAsync(GetCommonWordByIdQuery query, CancellationToken cancellationToken = default)
        => await correctionRepository.GetWordById(query.Language, query.Id, cancellationToken);

}
