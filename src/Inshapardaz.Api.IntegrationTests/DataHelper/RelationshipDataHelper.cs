﻿using System;
using System.Threading;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Repositories;

namespace Inshapardaz.Api.IntegrationTests.DataHelper
{
    public class RelationshipDataHelper
    {
        private readonly IRelationshipRepository _relationshipRepository;

        public RelationshipDataHelper(IRelationshipRepository relationshipRepository)
        {
            _relationshipRepository = relationshipRepository;
        }

        public Domain.Entities.WordRelation CreateRelationship(int dictionaryId, WordRelation relation)
        {
            return _relationshipRepository.AddRelationship(dictionaryId, relation, CancellationToken.None).Result;
        }

        public Domain.Entities.WordRelation GetRelationship(int dictionaryId, long relationshipId)
        {
            return _relationshipRepository.GetRelationshipById(dictionaryId, relationshipId, CancellationToken.None).Result;
        }

        public void DeleteRelationship(int dictionaryId, long relationshipId)
        {
            _relationshipRepository.DeleteRelationship(dictionaryId, relationshipId, CancellationToken.None).Wait();
        }
    }
}