using Inshapardaz.Domain.Adapters.Repositories;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Tools;

public class DeleteCommonWordRequest : RequestBase
{
    public DeleteCommonWordRequest(long wordId, string language)
    {
        WordId = wordId;
        Language = language;
    }

    public long WordId { get; }
    public string Language { get; }
}

public class DeleteCommonWordRequestHandler : RequestHandlerAsync<DeleteCommonWordRequest>
{
    private readonly ICommonWordsRepository _commonWordsRepository;

    public DeleteCommonWordRequestHandler(ICommonWordsRepository commonWordsRepository)
    {
        _commonWordsRepository = commonWordsRepository;
    }

    [AuthorizeAdmin(1)]
    public override async Task<DeleteCommonWordRequest> HandleAsync(DeleteCommonWordRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        await _commonWordsRepository.DeleteWord(command.Language, command.WordId, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
