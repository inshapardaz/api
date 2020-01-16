using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Dictionaries;

namespace Inshapardaz.Domain.Repositories.Dictionaries
{
    public interface IRelationshipRepository
    {
        Task<WordRelation> AddRelationship(int dictionaryId, WordRelation relation, CancellationToken cancellationToken);
        Task DeleteRelationship(int dictionaryId, long relationshipId, CancellationToken cancellationToken);
        Task UpdateRelationship(int dictionaryId, WordRelation relation, CancellationToken cancellationToken);
        Task<WordRelation> GetRelationshipById(int dictionaryId, long relationshipId, CancellationToken cancellationToken);
        Task<IEnumerable<WordRelation>> GetRelationshipFromWord(int dictionaryId, long sourceWordId, CancellationToken cancellationToken);
        Task<IEnumerable<WordRelation>> GetRelationshipToWord(int dictionaryId, long relatedWordId, CancellationToken cancellationToken);
    }
}
