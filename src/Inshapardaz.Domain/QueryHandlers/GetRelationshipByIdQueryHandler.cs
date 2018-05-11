using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetRelationshipByIdQueryHandler : QueryHandlerAsync<GetRelationshipByIdQuery, WordRelation>
    {
        private readonly IRelationshipRepository _relationshipRepository;

        public GetRelationshipByIdQueryHandler(IRelationshipRepository relationshipRepository)
        {
            _relationshipRepository = relationshipRepository;
        }

        public override async Task<WordRelation> ExecuteAsync(GetRelationshipByIdQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            return await _relationshipRepository.GetRelationshipById(query.DictionaryId, query.RelationshipId, cancellationToken);
        }
    }
}