using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Transliteration;

namespace Inshapardaz.Domain.Adapters.Repositories;

public interface ITransliterationRepository
{
    Task<IEnumerable<TransliterationEntryModel>> GetAllEntries(string language, CancellationToken cancellationToken);
    Task<Page<TransliterationEntryModel>> GetEntries(string language, string query, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<TransliterationEntryModel> GetEntryById(string language, long id, CancellationToken cancellationToken);
    Task<TransliterationEntryModel> GetEntryByRoman(string language, string roman, CancellationToken cancellationToken);
    Task<TransliterationEntryModel> AddEntry(TransliterationEntryModel entry, CancellationToken cancellationToken);
    Task<TransliterationEntryModel> UpdateEntry(TransliterationEntryModel entry, CancellationToken cancellationToken);
    Task DeleteEntry(string language, long id, CancellationToken cancellationToken);
}
