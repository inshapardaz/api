using Inshapardaz.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Adapters.Repositories
{
    public interface ICorrectionRepository
    {
        Task<Dictionary<string, string>> GetAutoCorrectionList(string language, CancellationToken cancellationToken);

        Task<Dictionary<string, string>> GetPunctuationList(string language,  CancellationToken cancellationToken);
        Task<Dictionary<string, string>> GetCorrectionList(string language,  CancellationToken cancellationToken);
    }
}
