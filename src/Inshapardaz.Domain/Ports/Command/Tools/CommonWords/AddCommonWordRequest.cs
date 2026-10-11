using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Helpers.Spelling;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Tools.CommonWords;

public class AddCommonWordRequest(CommonWordModel commonWordModel) : RequestBase
{
    public CommonWordModel CommonWordModel { get; } = commonWordModel;
    public CommonWordModel Result { get; set; }
}

public class AddCommonWordRequestHandler(ICommonWordsRepository commonWordsRepository, SpellingIndexCache spellingIndexCache)
    : RequestHandlerAsync<AddCommonWordRequest>
{
    [AuthorizeEditor(1)]
    public override async Task<AddCommonWordRequest> HandleAsync(AddCommonWordRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        command.Result = await commonWordsRepository.AddWord(command.CommonWordModel, cancellationToken);
        spellingIndexCache.Invalidate(command.CommonWordModel.Language);

        return await base.HandleAsync(command, cancellationToken);
    }
}
