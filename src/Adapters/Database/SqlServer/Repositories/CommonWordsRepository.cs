using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Adapters.Database.SqlServer.Repositories;

public class CommonWordsRepository : ICommonWordsRepository
{
    public Task<IEnumerable<string>> GetWordsForLanguage(string language, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<Page<CommonWordModel>> GetWords(string language, string query, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<CommonWordModel> GetWordById(string queryLanguage, long id, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<CommonWordModel> AddWord(CommonWordModel commonWordModel, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<CommonWordModel> UpdateWord(CommonWordModel commonWordModel, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task DeleteWord(string language, long id, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}
