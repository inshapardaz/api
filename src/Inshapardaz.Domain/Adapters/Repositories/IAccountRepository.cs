using Inshapardaz.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Repositories
{
    public interface IAccountRepository
    {
        Task<Page<AccountModel>> GetAccounts(int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<Page<AccountModel>> FindAccounts(string query, int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}
