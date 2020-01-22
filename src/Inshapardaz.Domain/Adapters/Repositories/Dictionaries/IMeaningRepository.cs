using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Dictionaries;

namespace Inshapardaz.Domain.Repositories.Dictionaries
{
    public interface IMeaningRepository
    {
        Task<MeaningModel> AddMeaning(int dictionaryId, long wordId, MeaningModel meaning, CancellationToken cancellationToken);
        Task DeleteMeaning(int dictionaryId, long wordId, long meaningId, CancellationToken cancellationToken);
        Task<MeaningModel> GetMeaningById(int dictionaryId, long wordId, long meaningId, CancellationToken cancellationToken);
        Task UpdateMeaning(int dictionaryId, IFormattable wordId, MeaningModel meaning, CancellationToken cancellationToken);
        Task<IEnumerable<MeaningModel>> GetMeaningByContext(int dictionaryId, long wordId, string context, CancellationToken cancellationToken);
        Task<IEnumerable<MeaningModel>> GetMeaningByWordId(int dictionaryId, long wordId, CancellationToken cancellationToken);
    }
}
