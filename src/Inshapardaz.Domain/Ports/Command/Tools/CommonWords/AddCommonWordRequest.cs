using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Tools.CommonWords;

public class AddCommonWordRequest(CommonWordModel commonWordModel) : RequestBase
{
    public CommonWordModel CommonWordModel { get; } = commonWordModel;
    public CommonWordModel Result { get; set; }
}

public class AddCommonWordRequestHandler(ICommonWordsRepository commonWordsRepository)
    : RequestHandlerAsync<AddCommonWordRequest>
{
    [AuthorizeAdmin(1)]
    public override async Task<AddCommonWordRequest> HandleAsync(AddCommonWordRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        command.Result = await commonWordsRepository.AddWord(command.CommonWordModel, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
