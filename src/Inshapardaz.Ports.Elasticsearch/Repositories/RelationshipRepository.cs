using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;

namespace Inshapardaz.Ports.Elasticsearch.Repositories
{
    public class RelationshipRepository : IRelationshipRepository
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public RelationshipRepository(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public Task<WordRelation> AddRelationship(int dictionaryId, WordRelation relation, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRelationship(int dictionaryId, long relationshipId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateRelationship(int dictionaryId, WordRelation relation, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<WordRelation> GetRelationshipById(int dictionaryId, long relationshipId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WordRelation>> GetRelationshipFromWord(int dictionaryId, long sourceWordId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WordRelation>> GetRelationshipToWord(int dictionaryId, long relatedWordId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
