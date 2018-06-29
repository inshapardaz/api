using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Dictionary;

namespace Inshapardaz.Domain.Repositories.Dictionary
{
    public interface IDictionaryRepository
    {
        Task<Entities.Dictionary.Dictionary> AddDictionary(Entities.Dictionary.Dictionary dictionary, CancellationToken cancellationToken);

        Task UpdateDictionary(int dictionaryId,  Entities.Dictionary.Dictionary dictionary, CancellationToken cancellationToken);

        Task DeleteDictionary(int dictionaryId, CancellationToken cancellationToken);

        Task<IEnumerable<Entities.Dictionary.Dictionary>> GetAllDictionaries(CancellationToken cancellationToken);
        Task<IEnumerable<Entities.Dictionary.Dictionary>> GetPublicDictionaries(CancellationToken cancellationToken);

        Task<IEnumerable<Entities.Dictionary.Dictionary>> GetAllDictionariesForUser(Guid userId, CancellationToken cancellationToken);

        Task<Entities.Dictionary.Dictionary> GetDictionaryById(int dictionaryId, CancellationToken cancellationToken);

        Task<IEnumerable<Entities.Dictionary.DictionaryDownload>> GetDictionaryDownloads(int dictionaryId, CancellationToken cancellationToken);

        Task<DictionaryDownload> GetDictionaryDownloadById(int dictionaryId, string mimeType, CancellationToken cancellationToken);

        Task<DictionaryDownload> AddDictionaryDownload(int dictionaryId, string mimeType, CancellationToken cancellationToken);
    }
}