using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Dictionaries;

namespace Inshapardaz.Domain.Repositories.Dictionaries
{
    public interface IMeaningRepository
    {
        Task<Meaning> AddMeaning(int dictionaryId, long wordId, Meaning meaning, CancellationToken cancellationToken);
        Task DeleteMeaning(int dictionaryId, long wordId, long meaningId, CancellationToken cancellationToken);
        Task<Meaning> GetMeaningById(int dictionaryId, long wordId, long meaningId, CancellationToken cancellationToken);
        Task UpdateMeaning(int dictionaryId, IFormattable wordId, Meaning meaning, CancellationToken cancellationToken);
        Task<IEnumerable<Meaning>> GetMeaningByContext(int dictionaryId, long wordId, string context, CancellationToken cancellationToken);
        Task<IEnumerable<Meaning>> GetMeaningByWordId(int dictionaryId, long wordId, CancellationToken cancellationToken);
    }
}
