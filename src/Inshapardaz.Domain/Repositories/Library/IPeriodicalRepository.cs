using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface IPeriodicalRepository
    {
        Task<Periodical> GetPeriodicalById(int periodicalId, CancellationToken cancellationToken);
        Task<Page<Periodical>> GetPeriodicals(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<Page<Periodical>> SearchPeriodicals(string query, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<Periodical> AddPeriodical(Periodical periodical, CancellationToken cancellationToken);
        Task UpdatePeriodical(Periodical periodical, CancellationToken cancellationToken);
        Task DeletePeriodical(int periodicalId, CancellationToken cancellationToken);
    }
}
