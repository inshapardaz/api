using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetRelationshipsByWordQueryHandler : QueryHandlerAsync<GetRelationshipsByWordQuery,
        IEnumerable<WordRelation>>
    {
        private readonly IRelationshipRepository _relationshipRepository;

        public GetRelationshipsByWordQueryHandler(IRelationshipRepository relationshipRepository)
        {
            _relationshipRepository = relationshipRepository;
        }

        public override async Task<IEnumerable<WordRelation>> ExecuteAsync(GetRelationshipsByWordQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _relationshipRepository.GetRelationshipFromWord(query.DictionaryId, query.WordId, cancellationToken);
        }
    }
}