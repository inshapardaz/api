using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers.Transliteration;
using Inshapardaz.Domain.Models.Transliteration;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Tools.Transliteration;

public class AddTransliterationEntryRequest(TransliterationEntryModel entry) : RequestBase
{
    public TransliterationEntryModel Entry { get; } = entry;
    public TransliterationEntryModel Result { get; set; }
}

public class AddTransliterationEntryRequestHandler(ITransliterationRepository transliterationRepository, TransliterationDictionaryCache dictionaryCache)
    : RequestHandlerAsync<AddTransliterationEntryRequest>
{
    [AuthorizeLanguageTools(1)]
    public override async Task<AddTransliterationEntryRequest> HandleAsync(AddTransliterationEntryRequest command, CancellationToken cancellationToken = default)
    {
        TransliterationEntryValidation.Normalise(command.Entry);
        if (await transliterationRepository.GetEntryByRoman(command.Entry.Language, command.Entry.Roman, cancellationToken) != null)
        {
            throw new ConflictException($"'{command.Entry.Roman}' is already in the dictionary.");
        }

        command.Result = await transliterationRepository.AddEntry(command.Entry, cancellationToken);
        dictionaryCache.Invalidate(command.Entry.Language);

        return await base.HandleAsync(command, cancellationToken);
    }
}

public class UpdateTransliterationEntryRequest(TransliterationEntryModel entry) : RequestBase
{
    public TransliterationEntryModel Entry { get; } = entry;
    public TransliterationEntryModel Result { get; set; }
}

public class UpdateTransliterationEntryRequestHandler(ITransliterationRepository transliterationRepository, TransliterationDictionaryCache dictionaryCache)
    : RequestHandlerAsync<UpdateTransliterationEntryRequest>
{
    [AuthorizeLanguageTools(1)]
    public override async Task<UpdateTransliterationEntryRequest> HandleAsync(UpdateTransliterationEntryRequest command, CancellationToken cancellationToken = default)
    {
        TransliterationEntryValidation.Normalise(command.Entry);
        _ = await transliterationRepository.GetEntryById(command.Entry.Language, command.Entry.Id, cancellationToken)
            ?? throw new NotFoundException();

        var clash = await transliterationRepository.GetEntryByRoman(command.Entry.Language, command.Entry.Roman, cancellationToken);
        if (clash != null && clash.Id != command.Entry.Id)
        {
            throw new ConflictException($"'{command.Entry.Roman}' is already in the dictionary.");
        }

        command.Result = await transliterationRepository.UpdateEntry(command.Entry, cancellationToken);
        dictionaryCache.Invalidate(command.Entry.Language);

        return await base.HandleAsync(command, cancellationToken);
    }
}

public class DeleteTransliterationEntryRequest(string language, long id) : RequestBase
{
    public string Language { get; } = language;
    public long Id { get; } = id;
}

public class DeleteTransliterationEntryRequestHandler(ITransliterationRepository transliterationRepository, TransliterationDictionaryCache dictionaryCache)
    : RequestHandlerAsync<DeleteTransliterationEntryRequest>
{
    [AuthorizeLanguageTools(1)]
    public override async Task<DeleteTransliterationEntryRequest> HandleAsync(DeleteTransliterationEntryRequest command, CancellationToken cancellationToken = default)
    {
        await transliterationRepository.DeleteEntry(command.Language, command.Id, cancellationToken);
        dictionaryCache.Invalidate(command.Language);

        return await base.HandleAsync(command, cancellationToken);
    }
}

// Adds many entries; an entry whose roman form already exists replaces that entry's native
// spelling, meaning and category. Invalid entries and repeats within the upload are skipped.
public class ImportTransliterationEntriesRequest(string language, IEnumerable<TransliterationEntryModel> entries) : RequestBase
{
    public string Language { get; } = language;
    public IEnumerable<TransliterationEntryModel> Entries { get; } = entries;
    public TransliterationImportResultModel Result { get; set; }
}

public class ImportTransliterationEntriesRequestHandler(ITransliterationRepository transliterationRepository, TransliterationDictionaryCache dictionaryCache)
    : RequestHandlerAsync<ImportTransliterationEntriesRequest>
{
    [AuthorizeLanguageTools(1)]
    public override async Task<ImportTransliterationEntriesRequest> HandleAsync(ImportTransliterationEntriesRequest command, CancellationToken cancellationToken = default)
    {
        TransliterationEntryValidation.ValidateLanguage(command.Language);

        var existing = (await transliterationRepository.GetAllEntries(command.Language, cancellationToken))
            .GroupBy(e => TransliterationDictionaryCache.RomanKey(e.Roman))
            .ToDictionary(g => g.Key, g => g.First());
        var seen = new HashSet<string>();
        var result = new TransliterationImportResultModel();

        foreach (var entry in command.Entries ?? [])
        {
            if (entry == null) { result.Skipped++; continue; }

            entry.Language = command.Language;
            entry.Roman = TransliterationDictionaryCache.RomanKey(entry.Roman);
            entry.Native = entry.Native?.Trim();
            if (entry.Roman.Length == 0 || string.IsNullOrEmpty(entry.Native) || !seen.Add(entry.Roman))
            {
                result.Skipped++;
                continue;
            }

            if (existing.TryGetValue(entry.Roman, out var current))
            {
                if (current.Native == entry.Native && current.Meaning == entry.Meaning && current.Category == entry.Category)
                {
                    result.Skipped++;
                    continue;
                }

                entry.Id = current.Id;
                await transliterationRepository.UpdateEntry(entry, cancellationToken);
                result.Updated++;
            }
            else
            {
                await transliterationRepository.AddEntry(entry, cancellationToken);
                result.Added++;
            }
        }

        dictionaryCache.Invalidate(command.Language);
        command.Result = result;
        return await base.HandleAsync(command, cancellationToken);
    }
}

internal static class TransliterationEntryValidation
{
    public static void ValidateLanguage(string language)
    {
        if (!TransliterationLanguages.TryGetScript(language, out _))
        {
            throw new BadRequestException($"Transliteration is not available for '{language}'. Available: {string.Join(", ", TransliterationLanguages.All)}.");
        }
    }

    public static void Normalise(TransliterationEntryModel entry)
    {
        ValidateLanguage(entry.Language);
        entry.Roman = TransliterationDictionaryCache.RomanKey(entry.Roman);
        entry.Native = entry.Native?.Trim();
        if (entry.Roman.Length == 0 || string.IsNullOrEmpty(entry.Native))
        {
            throw new BadRequestException("Both the roman and the native spelling are required.");
        }
    }
}
