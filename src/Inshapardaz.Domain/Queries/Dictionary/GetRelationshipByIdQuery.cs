using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries.Dictionary
{
    public class GetRelationshipByIdQuery : IQuery<WordRelation>
    {
        public GetRelationshipByIdQuery(int dictionaryId, long wordId, long relationRelationshipId)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
            RelationshipId = relationRelationshipId;
        }

        public int DictionaryId { get; }

        public long WordId { get; }

        public long RelationshipId { get; }
    }

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