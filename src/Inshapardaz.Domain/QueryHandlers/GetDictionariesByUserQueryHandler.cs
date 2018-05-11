using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionariesByUserQueryHandler : QueryHandlerAsync<GetDictionariesByUserQuery, IEnumerable<Dictionary>>
    {
        private readonly IDictionaryRepository _dictionaryRepository;

        public GetDictionariesByUserQueryHandler(IDictionaryRepository dictionaryRepository)
        {
            _dictionaryRepository = dictionaryRepository;
        }

        public override async Task<IEnumerable<Dictionary>> ExecuteAsync(GetDictionariesByUserQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query.UserId != Guid.Empty)
            {
                return await _dictionaryRepository.GetAllDictionariesForUser(query.UserId, cancellationToken);
            }

            return await _dictionaryRepository.GetPublicDictionaries(cancellationToken);
        }
    }
}