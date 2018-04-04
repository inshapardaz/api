using System;
using Paramore.Darker;
using Inshapardaz.Domain.Queries;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Elasticsearch;
using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetRelationshipToWordQueryHandler : QueryHandlerAsync<GetRelationshipToWordQuery,
        IEnumerable<WordRelation>>
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public GetRelationshipToWordQueryHandler(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public override async Task<IEnumerable<WordRelation>> ExecuteAsync(GetRelationshipToWordQuery query,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            //return await _database.WordRelation
            //    .Include(r => r.RelatedWord)
            //    .Include(r => r.SourceWord)
            //    .Where(t => t.RelatedWordId == query.WordId)
            //    .ToListAsync(cancellationToken);
            throw new NotImplementedException();
        }
    }
}