using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries.Dictionary
{
    public class GetWordMeaningByIdQuery : IQuery<Meaning>
    {
        public GetWordMeaningByIdQuery(int dictionaryId, long wordId, long meaningId)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
            MeaningId = meaningId;
        }

        public int DictionaryId { get; }

        public long WordId { get; }

        public long MeaningId { get; }
    }

    public class GetWordMeaningByIdQueryHandler : QueryHandlerAsync<GetWordMeaningByIdQuery, Meaning>
    {
        private readonly IMeaningRepository _meaningRepository;

        public GetWordMeaningByIdQueryHandler(IMeaningRepository meaningRepository)
        {
            _meaningRepository = meaningRepository;
        }

        public override async Task<Meaning> ExecuteAsync(GetWordMeaningByIdQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _meaningRepository.GetMeaningById(query.DictionaryId, query.WordId, query.MeaningId, cancellationToken);
        }
    }
}