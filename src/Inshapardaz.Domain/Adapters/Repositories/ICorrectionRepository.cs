using Inshapardaz.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Adapters.Repositories
{
    public interface ICorrectionRepository
    {
        Task<Dictionary<string, string>> GetAllCorrections(string language, string profile,  CancellationToken cancellationToken);
        Task<Page<CorrectionModel>> GetCorrectionList(string language, string query, string profile, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<CorrectionModel> GetCorrection(string language, string profile, string incorrectText, CancellationToken cancellationToken);
        Task<CorrectionModel> AddCorrection(CorrectionModel correction, CancellationToken cancellationToken);
        Task<CorrectionModel> UpdateCorrection(CorrectionModel correction, CancellationToken cancellationToken);
        Task DeleteCorrection(string language, string profile, string incorrectText, CancellationToken cancellationToken);
    }
}
