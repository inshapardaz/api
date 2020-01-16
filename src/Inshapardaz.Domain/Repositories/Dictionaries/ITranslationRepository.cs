using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionaries;

namespace Inshapardaz.Domain.Repositories.Dictionaries
{
    public interface ITranslationRepository
    {
        Task<Translation> AddTranslation(int dictionaryId, long wordId, Translation translation, CancellationToken cancellationToken);
        Task DeleteTranslation(int dictionaryId, long wordId, long translationId, CancellationToken cancellationToken);
        Task UpdateTranslation(int dictionaryId, long wordId, Translation translation, CancellationToken cancellationToken);
        Task<Translation> GetTranslationById(int dictionaryId, long wordId, long translationId, CancellationToken cancellationToken);
        Task<IEnumerable<Translation>> GetTranslationsByLanguage(int dictionaryId, long wordId, Languages language, CancellationToken cancellationToken);
        Task<IEnumerable<Translation>> GetTranslationsByWordId(int dictionaryId, long wordId, CancellationToken cancellationToken);
    }
}
