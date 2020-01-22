using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Dictionaries;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Ports.Database.Repositories.Dictionaries
{
    public class RelationshipRepository : IRelationshipRepository
    {
        private readonly IDatabaseContext _databaseContext;

        public RelationshipRepository(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<WordRelation> AddRelationship(int dictionaryId, WordRelation relation, CancellationToken cancellationToken)
        {
            var wordRelation = relation.Map();
            _databaseContext.WordRelation.Add(wordRelation);
            await _databaseContext.SaveChangesAsync(cancellationToken);

            return wordRelation.Map();
        }

        public async Task DeleteRelationship(int dictionaryId, long relationshipId, CancellationToken cancellationToken)
        {
            var relation = await _databaseContext.WordRelation
                                                 .SingleOrDefaultAsync(r => r.SourceWord.DictionaryId == dictionaryId && 
                                                                            r.Id == relationshipId,
                                                                       cancellationToken);

            if (relation == null)
            {
                throw new NotFoundException();
            }

            _databaseContext.WordRelation.Remove(relation);
            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateRelationship(int dictionaryId, WordRelation relation, CancellationToken cancellationToken)
        {
            var oldRelation = await _databaseContext.WordRelation
                                                 .SingleOrDefaultAsync(r => r.SourceWord.DictionaryId == dictionaryId &&
                                                                            r.Id == relation.Id,
                                                                       cancellationToken);

            if (oldRelation == null)
            {
                throw new NotFoundException();
            }

            oldRelation.RelatedWordId = relation.RelatedWordId;
            oldRelation.RelationType = relation.RelationType;

            await _databaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<WordRelation> GetRelationshipById(int dictionaryId, long relationshipId, CancellationToken cancellationToken)
        {
            var relation  = await _databaseContext.WordRelation
                                  .Include(r => r.SourceWord)
                                  .Include(r => r.RelatedWord)
                                  .SingleOrDefaultAsync(t => t.Id == relationshipId, cancellationToken);
            return relation.Map();
        }

        public async Task<IEnumerable<WordRelation>> GetRelationshipFromWord(int dictionaryId, long sourceWordId, CancellationToken cancellationToken)
        {
            return await _databaseContext.WordRelation
                                  .Include(r => r.RelatedWord)
                                  .Include(r => r.SourceWord)
                                  .Where(t => t.SourceWordId == sourceWordId)
                                  .Select(r => r.Map())
                                  .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<WordRelation>> GetRelationshipToWord(int dictionaryId, long relatedWordId, CancellationToken cancellationToken)
        {
            return await _databaseContext.WordRelation
                                        .Include(r => r.RelatedWord)
                                        .Include(r => r.SourceWord)
                                        .Where(t => t.RelatedWordId == relatedWordId)
                                        .Select(r => r.Map())
                                        .ToListAsync(cancellationToken);
        }
    }
}
