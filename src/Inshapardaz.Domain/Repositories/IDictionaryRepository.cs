using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Domain.Repositories
{
    public interface IDictionaryRepository
    {
        Task<Dictionary> AddDictionary(Dictionary dictionary, CancellationToken cancellationToken);

        Task UpdateDictionary(int dictionaryId,  Dictionary dictionary, CancellationToken cancellationToken);

        Task DeleteDictionary(int dictionaryId, CancellationToken cancellationToken);

        Task<IEnumerable<Dictionary>> GetAllDictionaries(CancellationToken cancellationToken);
        Task<IEnumerable<Dictionary>> GetPublicDictionaries(CancellationToken cancellationToken);

        Task<IEnumerable<Dictionary>> GetAllDictionariesForUser(Guid userId, CancellationToken cancellationToken);

        Task<Dictionary> GetDictionaryById(int dictionaryId, CancellationToken cancellationToken);
    }
}