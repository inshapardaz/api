using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries.Dictionary
{
    public class GetDictionariesQuery : IQuery<IEnumerable<Entities.Dictionary.Dictionary>>
    {
    }

    public class GetDictionariesQueryHandler : QueryHandlerAsync<GetDictionariesQuery,
        IEnumerable<Entities.Dictionary.Dictionary>>
    {
        private readonly IDictionaryRepository _dictionaryRepository;

        public GetDictionariesQueryHandler(IDictionaryRepository dictionaryRepository)
        {
            _dictionaryRepository = dictionaryRepository;
        }

        public override async Task<IEnumerable<Entities.Dictionary.Dictionary>> ExecuteAsync(GetDictionariesQuery query,
                                                                         CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _dictionaryRepository.GetAllDictionaries(cancellationToken);
        }
    }
}