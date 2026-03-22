using Inshapardaz.Domain.Adapters.Repositories;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Tools;

public class DeleteCommonWordRequest(long wordId, string language) : RequestBase
{
    public long WordId { get; } = wordId;
    public string Language { get; } = language;
}

public class DeleteCommonWordRequestHandler(ICommonWordsRepository commonWordsRepository)
    : RequestHandlerAsync<DeleteCommonWordRequest>
{
    [AuthorizeAdmin(1)]
    public override async Task<DeleteCommonWordRequest> HandleAsync(DeleteCommonWordRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        await commonWordsRepository.DeleteWord(command.Language, command.WordId, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
