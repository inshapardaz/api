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
    public class GetRelationshipToWordQueryHandler : QueryHandlerAsync<GetRelationshipToWordQuery,
        IEnumerable<WordRelation>>
    {
        private readonly IRelationshipRepository _relationshipRepository;

        public GetRelationshipToWordQueryHandler(IRelationshipRepository relationshipRepository)
        {
            _relationshipRepository = relationshipRepository;
        }

        public override async Task<IEnumerable<WordRelation>> ExecuteAsync(GetRelationshipToWordQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _relationshipRepository.GetRelationshipToWord(query.DictionaryId, query.WordId, cancellationToken);
        }
    }
}