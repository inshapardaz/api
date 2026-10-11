using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Tools.Dictionaries;

// Creates an empty dictionary owned by the current user, to be filled article by article.
public class AddDictionaryRequest(DictionaryModel dictionary) : RequestBase
{
    public DictionaryModel Dictionary { get; } = dictionary;
    public DictionaryModel Result { get; set; }
}

public class AddDictionaryRequestHandler(IDictionaryRepository dictionaryRepository, IUserHelper userHelper)
    : RequestHandlerAsync<AddDictionaryRequest>
{
    [AuthorizeLanguageTools(1)]
    public override async Task<AddDictionaryRequest> HandleAsync(AddDictionaryRequest command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Dictionary.Name))
        {
            throw new BadRequestException("A dictionary needs a name.");
        }

        command.Dictionary.Format = DictionaryFormat.Manual;
        command.Dictionary.EntryCount = 0;
        command.Dictionary.OwnerId = userHelper.AccountId;
        command.Dictionary.OriginalFileName = null;
        command.Result = await dictionaryRepository.AddDictionary(command.Dictionary, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
