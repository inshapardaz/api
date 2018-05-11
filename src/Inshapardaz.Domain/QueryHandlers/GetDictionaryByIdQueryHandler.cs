using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionaryByIdQueryHandler : QueryHandlerAsync<GetDictionaryByIdQuery, Dictionary>
    {
        private readonly IDictionaryRepository _dictionaryRepository;

        public GetDictionaryByIdQueryHandler(IDictionaryRepository dictionaryRepository)
        {
            _dictionaryRepository = dictionaryRepository;
        }

        public override async Task<Dictionary> ExecuteAsync(GetDictionaryByIdQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _dictionaryRepository.GetDictionaryById(query.DictionaryId, cancellationToken);
        }
    }
}