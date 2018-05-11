using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionariesQueryHandler : QueryHandlerAsync<GetDictionariesQuery,
        IEnumerable<Dictionary>>
    {
        private readonly IDictionaryRepository _dictionaryRepository;

        public GetDictionariesQueryHandler(IDictionaryRepository dictionaryRepository)
        {
            _dictionaryRepository = dictionaryRepository;
        }

        public override async Task<IEnumerable<Dictionary>> ExecuteAsync(GetDictionariesQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _dictionaryRepository.GetAllDictionaries(cancellationToken);
        }
    }
}