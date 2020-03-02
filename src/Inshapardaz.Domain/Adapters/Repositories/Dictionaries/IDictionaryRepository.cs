using Inshapardaz.Domain.Models.Dictionaries;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Repositories.Dictionaries
{
    public interface IDictionaryRepository
    {
        Task<DictionaryModel> AddDictionary(DictionaryModel dictionary, CancellationToken cancellationToken);

        Task UpdateDictionary(DictionaryModel dictionary, CancellationToken cancellationToken);

        Task DeleteDictionary(int dictionaryId, CancellationToken cancellationToken);

        Task<IEnumerable<DictionaryModel>> GetAllDictionaries(CancellationToken cancellationToken);

        Task<IEnumerable<DictionaryModel>> GetPublicDictionaries(CancellationToken cancellationToken);

        Task<IEnumerable<DictionaryModel>> GetAllDictionariesForUser(Guid userId, CancellationToken cancellationToken);

        Task<DictionaryModel> GetDictionaryById(int dictionaryId, CancellationToken cancellationToken);

        Task<IEnumerable<DictionaryDownload>> GetDictionaryDownloads(int dictionaryId, CancellationToken cancellationToken);

        Task<DictionaryDownload> GetDictionaryDownloadById(int dictionaryId, string mimeType, CancellationToken cancellationToken);

        Task<DictionaryDownload> AddDictionaryDownload(int dictionaryId, string mimeType, CancellationToken cancellationToken);
    }
}
