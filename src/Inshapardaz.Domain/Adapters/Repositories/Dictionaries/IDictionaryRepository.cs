using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Dictionaries;

namespace Inshapardaz.Domain.Repositories.Dictionaries
{
    public interface IDictionaryRepository
    {
        Task<Models.Dictionaries.DictionaryModel> AddDictionary(Models.Dictionaries.DictionaryModel dictionary, CancellationToken cancellationToken);

        Task UpdateDictionary(int dictionaryId,  Models.Dictionaries.DictionaryModel dictionary, CancellationToken cancellationToken);

        Task DeleteDictionary(int dictionaryId, CancellationToken cancellationToken);

        Task<IEnumerable<Models.Dictionaries.DictionaryModel>> GetAllDictionaries(CancellationToken cancellationToken);
        Task<IEnumerable<Models.Dictionaries.DictionaryModel>> GetPublicDictionaries(CancellationToken cancellationToken);

        Task<IEnumerable<Models.Dictionaries.DictionaryModel>> GetAllDictionariesForUser(Guid userId, CancellationToken cancellationToken);

        Task<Models.Dictionaries.DictionaryModel> GetDictionaryById(int dictionaryId, CancellationToken cancellationToken);

        Task<IEnumerable<Models.Dictionaries.DictionaryDownload>> GetDictionaryDownloads(int dictionaryId, CancellationToken cancellationToken);

        Task<DictionaryDownload> GetDictionaryDownloadById(int dictionaryId, string mimeType, CancellationToken cancellationToken);

        Task<DictionaryDownload> AddDictionaryDownload(int dictionaryId, string mimeType, CancellationToken cancellationToken);
    }
}
