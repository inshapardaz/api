using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Dictionaries;

namespace Inshapardaz.Domain.Repositories.Dictionaries
{
    public interface IWordRepository
    {
        Task<WordModel> AddWord(int dictionaryId, WordModel word, CancellationToken cancellationToken);
        Task DeleteWord(int dictionaryId, long wordId, CancellationToken cancellationToken);
        Task UpdateWord(int dictionaryId, WordModel word, CancellationToken cancellationToken);
        Task<WordModel> GetWordById(int dictionaryId, long wordId, CancellationToken cancellationToken);
        Task<WordModel> GetWordByTitle(int dictionaryId, string title, CancellationToken cancellationToken);
        Task<Page<WordModel>> GetWordsById(int dictionaryId, IEnumerable<long> wordIds, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<WordModel>> GetWordsByTitles(int dictionaryId, IEnumerable<string> titles, CancellationToken cancellationToken);
        Task<Page<WordModel>> GetWordsContaining(int dictionaryId, string searchTerm, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<Page<WordModel>> GetWords(int dictionaryId, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<int> GetWordCountByDictionary(int dictionaryId, CancellationToken cancellationToken);
        Task<Page<WordModel>> GetWordsStartingWith(int dictionaryId, string startingWith, int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}
