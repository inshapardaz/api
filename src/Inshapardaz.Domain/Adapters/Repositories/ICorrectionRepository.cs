using Inshapardaz.Domain.Models;

namespace Inshapardaz.Domain.Adapters.Repositories;

public interface ICorrectionRepository
{
    Task<IEnumerable<CorrectionModel>> GetAllCorrections(string language, string profile, CancellationToken cancellationToken);
    Task<Page<CorrectionModel>> GetCorrectionList(string language, string query, string profile, int pageNumber, int pageSize, CorrectionSortByType sortBy, CancellationToken cancellationToken);
    Task<CorrectionModel> GetCorrection(string language, string profile, long id, CancellationToken cancellationToken);
    Task<CorrectionModel> AddCorrection(CorrectionModel correction, CancellationToken cancellationToken);
    Task<CorrectionModel> UpdateCorrection(CorrectionModel correction, CancellationToken cancellationToken);
    Task DeleteCorrection(long id, CancellationToken cancellationToken);

    // Adds each count to that correction's Usage; ids outside the language/profile are ignored.
    Task IncrementUsage(string language, string profile, IEnumerable<CorrectionUsageModel> usages, CancellationToken cancellationToken);
    Task<IEnumerable<CorrectionModel>> GetAllCorrections(CancellationToken cancellationToken);
}
