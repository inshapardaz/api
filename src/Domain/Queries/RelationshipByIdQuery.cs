using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class RelationshipByIdQuery : IQuery<RelationshipByIdQuery.Response>
    {
        public long Id { get; set; }

        public class Response
        {
            public WordRelation Relation { get; set; }
        }
    }
}