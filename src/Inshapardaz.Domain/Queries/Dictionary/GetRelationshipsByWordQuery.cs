using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries.Dictionary
{
    public class GetRelationshipsByWordQuery : IQuery<IEnumerable<WordRelation>>
    {
        public GetRelationshipsByWordQuery(int dictionaryId, long wordId)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
        }

        public int DictionaryId { get; }

        public long WordId { get; }
    }

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