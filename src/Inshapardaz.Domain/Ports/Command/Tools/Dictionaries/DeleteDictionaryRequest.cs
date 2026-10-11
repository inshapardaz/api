using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Helpers.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Tools.Dictionaries;

// Deletes a dictionary with all of its articles.
public class DeleteDictionaryRequest(long dictionaryId) : RequestBase
{
    public long DictionaryId { get; } = dictionaryId;
}

public class DeleteDictionaryRequestHandler(IDictionaryRepository dictionaryRepository, IUserHelper userHelper)
    : RequestHandlerAsync<DeleteDictionaryRequest>
{
    [Authorize(1)]
    public override async Task<DeleteDictionaryRequest> HandleAsync(DeleteDictionaryRequest command, CancellationToken cancellationToken = default)
    {
        var dictionary = await dictionaryRepository.GetDictionaryById(command.DictionaryId, cancellationToken);
        if (dictionary != null)
        {
            DictionaryPermissions.EnsureCanWrite(userHelper, dictionary);
            await dictionaryRepository.DeleteDictionary(command.DictionaryId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
