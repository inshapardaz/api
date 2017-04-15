using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class RelationshipByIdQuery : IQueryRequest<RelationshipByIdQuery.Response>
    {
        public long Id { get; set; }

        public class Response : IQueryResponse
        {
            public WordRelation Relation { get; set; }
        }
    }
}