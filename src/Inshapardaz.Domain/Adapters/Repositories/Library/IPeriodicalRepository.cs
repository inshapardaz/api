using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Repositories.Library
{
    public interface IPeriodicalRepository
    {
        Task<PeriodicalModel> GetPeriodicalById(int libraryId, int periodicalId, CancellationToken cancellationToken);

        Task<Page<PeriodicalModel>> GetPeriodicals(int libraryId, string query, int pageNumber, int pageSize, PeriodicalFilter filter, PeriodicalSortByType sortBy, SortDirection direction, CancellationToken cancellationToken);

        Task<PeriodicalModel> AddPeriodical(int libraryId, PeriodicalModel periodical, CancellationToken cancellationToken);

        Task<PeriodicalModel> UpdatePeriodical(int libraryId, PeriodicalModel periodical, CancellationToken cancellationToken);

        Task DeletePeriodical(int libraryId, int periodicalId, CancellationToken cancellationToken);
        Task UpdatePeriodicalImage(int libraryId, int periodicalId, long imageId, CancellationToken cancellationToken);
    }
}
