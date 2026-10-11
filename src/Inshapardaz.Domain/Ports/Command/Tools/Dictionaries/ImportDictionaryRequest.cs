using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Tools.Dictionaries;

// Imports an uploaded StarDict/DSL dictionary as a new dictionary owned by the current user.
public class ImportDictionaryRequest(Stream file, string fileName) : RequestBase
{
    public Stream File { get; } = file;
    public string FileName { get; } = fileName;

    // Optional overrides for what the file's own metadata says.
    public string Name { get; set; }
    public string SourceLanguage { get; set; }
    public string TargetLanguage { get; set; }

    public DictionaryModel Result { get; set; }
}

public class ImportDictionaryRequestHandler(
    IDictionaryRepository dictionaryRepository,
    IReadDictionaryFiles dictionaryFileReader,
    IUserHelper userHelper)
    : RequestHandlerAsync<ImportDictionaryRequest>
{
    [AuthorizeLanguageTools(1)]
    public override async Task<ImportDictionaryRequest> HandleAsync(ImportDictionaryRequest command, CancellationToken cancellationToken = default)
    {
        var parsed = await dictionaryFileReader.ReadAsync(command.File, command.FileName, cancellationToken);
        if (parsed.Articles.Count == 0)
        {
            throw new BadRequestException("The dictionary has no articles.");
        }

        var info = parsed.Info;
        info.Name = string.IsNullOrWhiteSpace(command.Name) ? info.Name : command.Name.Trim();
        info.SourceLanguage = string.IsNullOrWhiteSpace(command.SourceLanguage) ? info.SourceLanguage : command.SourceLanguage.Trim();
        info.TargetLanguage = string.IsNullOrWhiteSpace(command.TargetLanguage) ? info.TargetLanguage : command.TargetLanguage.Trim();
        info.OwnerId = userHelper.AccountId;
        info.EntryCount = 0;

        var dictionary = await dictionaryRepository.AddDictionary(info, cancellationToken);
        try
        {
            await dictionaryRepository.AddArticles(dictionary.Id, parsed.Articles, cancellationToken);
        }
        catch
        {
            // Don't leave an empty or half-filled dictionary behind.
            await dictionaryRepository.DeleteDictionary(dictionary.Id, CancellationToken.None);
            throw;
        }

        await dictionaryRepository.UpdateEntryCount(dictionary.Id, cancellationToken);
        command.Result = await dictionaryRepository.GetDictionaryById(dictionary.Id, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
