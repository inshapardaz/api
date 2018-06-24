using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries.Dictionary
{
    public class GetDictionariesByUserQuery : IQuery<IEnumerable<Entities.Dictionary.Dictionary>>
    {
        public Guid UserId { get; set; }
    }

    public class GetDictionariesByUserQueryHandler : QueryHandlerAsync<GetDictionariesByUserQuery, IEnumerable<Entities.Dictionary.Dictionary>>
    {
        private readonly IDictionaryRepository _dictionaryRepository;

        public GetDictionariesByUserQueryHandler(IDictionaryRepository dictionaryRepository)
        {
            _dictionaryRepository = dictionaryRepository;
        }

        public override async Task<IEnumerable<Entities.Dictionary.Dictionary>> ExecuteAsync(GetDictionariesByUserQuery query,
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