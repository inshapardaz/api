using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers.Dictionaries;
using Inshapardaz.Domain.Models.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Tools.Dictionaries;

// Changes a dictionary's name, description and languages.
public class UpdateDictionaryRequest(DictionaryModel dictionary) : RequestBase
{
    public DictionaryModel Dictionary { get; } = dictionary;
    public DictionaryModel Result { get; set; }
}

public class UpdateDictionaryRequestHandler(IDictionaryRepository dictionaryRepository, IUserHelper userHelper)
    : RequestHandlerAsync<UpdateDictionaryRequest>
{
    [Authorize(1)]
    public override async Task<UpdateDictionaryRequest> HandleAsync(UpdateDictionaryRequest command, CancellationToken cancellationToken = default)
    {
        var existing = await dictionaryRepository.GetDictionaryById(command.Dictionary.Id, cancellationToken)
            ?? throw new NotFoundException();
        DictionaryPermissions.EnsureCanWrite(userHelper, existing);

        if (string.IsNullOrWhiteSpace(command.Dictionary.Name))
        {
            throw new BadRequestException("A dictionary needs a name.");
        }

        existing.Name = command.Dictionary.Name.Trim();
        existing.Description = command.Dictionary.Description;
        existing.SourceLanguage = command.Dictionary.SourceLanguage;
        existing.TargetLanguage = command.Dictionary.TargetLanguage;
        command.Result = await dictionaryRepository.UpdateDictionary(existing, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
