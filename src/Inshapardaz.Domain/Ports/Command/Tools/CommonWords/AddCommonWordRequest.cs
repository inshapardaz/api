using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Tools.CommonWords;

public class AddCommonWordRequest : RequestBase
{
    public AddCommonWordRequest(CommonWordModel commonWordModel)
    {
        CommonWordModel = commonWordModel;
    }

    public CommonWordModel CommonWordModel { get; }
    public CommonWordModel Result { get; set; }
}

public class AddCommonWordRequestHandler : RequestHandlerAsync<AddCommonWordRequest>
{
    private readonly ICommonWordsRepository _commonWordsRepository;

    public AddCommonWordRequestHandler(ICommonWordsRepository commonWordsRepository)
    {
        _commonWordsRepository = commonWordsRepository;
    }

    [AuthorizeAdmin(1)]
    public override async Task<AddCommonWordRequest> HandleAsync(AddCommonWordRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        command.Result = await _commonWordsRepository.AddWord(command.CommonWordModel, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
