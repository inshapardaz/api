using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Domain.Adapters.Repositories;

public interface ICommonWordsRepository
{
    Task<IEnumerable<string>> GetWordsForLanguage(string language, CancellationToken cancellationToken);
    Task<Page<CommonWordModel>> GetWords(string language, string query, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<CommonWordModel> GetWordById(string language, long id, CancellationToken cancellationToken);
    Task<CommonWordModel> AddWord(CommonWordModel commonWordModel, CancellationToken cancellationToken);
    Task<CommonWordModel> UpdateWord(CommonWordModel commonWordModel, CancellationToken cancellationToken);
    Task DeleteWord(string language, long id, CancellationToken cancellationToken);
}
