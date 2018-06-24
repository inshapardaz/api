using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries.Dictionary
{
    public class GetDictionaryByIdQuery : IQuery<Entities.Dictionary.Dictionary>
    {

        public int DictionaryId { get; set; }
    }

    public class GetDictionaryByIdQueryHandler : QueryHandlerAsync<GetDictionaryByIdQuery, Entities.Dictionary.Dictionary>
    {
        private readonly IDictionaryRepository _dictionaryRepository;

        public GetDictionaryByIdQueryHandler(IDictionaryRepository dictionaryRepository)
        {
            _dictionaryRepository = dictionaryRepository;
        }

        public override async Task<Entities.Dictionary.Dictionary> ExecuteAsync(GetDictionaryByIdQuery query,
                                                            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _dictionaryRepository.GetDictionaryById(query.DictionaryId, cancellationToken);
        }
    }
}