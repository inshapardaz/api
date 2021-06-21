using Inshapardaz.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Adapters.Repositories
{
    public interface ICorrectionRepository
    {
        Task<IEnumerable<Correction>> GetCorrectionForLanguage(string language, CancellationToken cancellationToken);
    }
}
