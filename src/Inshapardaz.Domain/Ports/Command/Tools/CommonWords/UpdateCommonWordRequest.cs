using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Tools;

public class UpdateCommonWordRequest : RequestBase
{
    public UpdateCommonWordRequest(CommonWordModel commonWordModel)
    {
        WordModel = commonWordModel;
    }

    public CommonWordModel WordModel { get; }
    public RequestResult Result { get; set; } = new RequestResult();
    public class RequestResult
    {
        public CommonWordModel WordModel { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateCommonWordRequestHandler : RequestHandlerAsync<UpdateCommonWordRequest>
{
    private readonly ICommonWordsRepository _commonWordsRepository;

    public UpdateCommonWordRequestHandler(ICommonWordsRepository commonWordsRepository)
    {
        _commonWordsRepository = commonWordsRepository;
    }

    [AuthorizeAdmin(1)]
    public override async Task<UpdateCommonWordRequest> HandleAsync(UpdateCommonWordRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await _commonWordsRepository.GetWordById(command.WordModel.Language, command.WordModel.Id, cancellationToken);

        if (result == null)
        {
            command.Result.WordModel = await _commonWordsRepository.AddWord(command.WordModel, cancellationToken);
            command.Result.HasAddedNew = true;
        }
        else
        {
            command.Result.WordModel = await _commonWordsRepository.UpdateWord(command.WordModel, cancellationToken); ;
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
