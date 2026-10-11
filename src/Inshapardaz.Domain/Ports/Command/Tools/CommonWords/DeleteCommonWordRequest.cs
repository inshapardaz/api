using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Helpers.Spelling;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Tools;

public class DeleteCommonWordRequest(long wordId, string language) : RequestBase
{
    public long WordId { get; } = wordId;
    public string Language { get; } = language;
}

public class DeleteCommonWordRequestHandler(ICommonWordsRepository commonWordsRepository, SpellingIndexCache spellingIndexCache)
    : RequestHandlerAsync<DeleteCommonWordRequest>
{
    [AuthorizeLanguageTools(1)]
    public override async Task<DeleteCommonWordRequest> HandleAsync(DeleteCommonWordRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        await commonWordsRepository.DeleteWord(command.Language, command.WordId, cancellationToken);
        spellingIndexCache.Invalidate(command.Language);

        return await base.HandleAsync(command, cancellationToken);
    }
}
