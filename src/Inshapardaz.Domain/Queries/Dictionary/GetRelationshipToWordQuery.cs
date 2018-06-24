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
    public class GetRelationshipToWordQuery : IQuery<IEnumerable<WordRelation>>
    {
        public GetRelationshipToWordQuery(int dictionaryId, long wordId)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
        }

        public int DictionaryId { get; set; }
        public long WordId { get; }
    }

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