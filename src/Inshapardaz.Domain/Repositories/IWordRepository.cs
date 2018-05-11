using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Domain.Repositories
{
    public interface IWordRepository
    {
        Task<Word> AddWord(int dictionaryId, Word word, CancellationToken cancellationToken);
        Task DeleteWord(int dictionaryId, long wordId, CancellationToken cancellationToken);
        Task UpdateWord(int dictionaryId, Word word, CancellationToken cancellationToken);
        Task<Word> GetWordById(int dictionaryId, long wordId, CancellationToken cancellationToken);
        Task<Word> GetWordByTitle(int dictionaryId, string title, CancellationToken cancellationToken);
        Task<Page<Word>> GetWordsById(int dictionaryId, IEnumerable<long> wordIds, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<Word>> GetWordsByTitles(int dictionaryId, IEnumerable<string> titles, CancellationToken cancellationToken);
        Task<Page<Word>> GetWordsContaining(int dictionaryId, string searchTerm, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<Page<Word>> GetWords(int dictionaryId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}
