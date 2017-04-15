using System.Collections.Generic;
using Darker;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.QueryHandlers;

namespace Inshapardaz.Domain.Queries
{
    public class RelationshipByWordIdQuery : IQueryRequest<RelationshipByWordIdQuery.Response>
    {
        public int WordId { get; set; }

        public class Response : IQueryResponse
        {
            public IEnumerable<WordRelation> Relations { get; set; }
        }
    }
}