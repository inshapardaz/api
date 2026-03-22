using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Tools;

public class UpdateCommonWordRequest(CommonWordModel commonWordModel) : RequestBase
{
    public CommonWordModel WordModel { get; } = commonWordModel;
    public RequestResult Result { get; set; } = new RequestResult();
    public class RequestResult
    {
        public CommonWordModel WordModel { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateCommonWordRequestHandler(ICommonWordsRepository commonWordsRepository)
    : RequestHandlerAsync<UpdateCommonWordRequest>
{
    [AuthorizeAdmin(1)]
    public override async Task<UpdateCommonWordRequest> HandleAsync(UpdateCommonWordRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await commonWordsRepository.GetWordById(command.WordModel.Language, command.WordModel.Id, cancellationToken);

        if (result == null)
        {
            command.Result.WordModel = await commonWordsRepository.AddWord(command.WordModel, cancellationToken);
            command.Result.HasAddedNew = true;
        }
        else
        {
            command.Result.WordModel = await commonWordsRepository.UpdateWord(command.WordModel, cancellationToken); ;
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
