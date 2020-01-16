using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Dictionaries;

namespace Inshapardaz.Domain.Repositories.Dictionaries
{
    public interface IDictionaryRepository
    {
        Task<Entities.Dictionaries.Dictionary> AddDictionary(Entities.Dictionaries.Dictionary dictionary, CancellationToken cancellationToken);

        Task UpdateDictionary(int dictionaryId,  Entities.Dictionaries.Dictionary dictionary, CancellationToken cancellationToken);

        Task DeleteDictionary(int dictionaryId, CancellationToken cancellationToken);

        Task<IEnumerable<Entities.Dictionaries.Dictionary>> GetAllDictionaries(CancellationToken cancellationToken);
        Task<IEnumerable<Entities.Dictionaries.Dictionary>> GetPublicDictionaries(CancellationToken cancellationToken);

        Task<IEnumerable<Entities.Dictionaries.Dictionary>> GetAllDictionariesForUser(Guid userId, CancellationToken cancellationToken);

        Task<Entities.Dictionaries.Dictionary> GetDictionaryById(int dictionaryId, CancellationToken cancellationToken);

        Task<IEnumerable<Entities.Dictionaries.DictionaryDownload>> GetDictionaryDownloads(int dictionaryId, CancellationToken cancellationToken);

        Task<DictionaryDownload> GetDictionaryDownloadById(int dictionaryId, string mimeType, CancellationToken cancellationToken);

        Task<DictionaryDownload> AddDictionaryDownload(int dictionaryId, string mimeType, CancellationToken cancellationToken);
    }
}
