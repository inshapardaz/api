using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
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