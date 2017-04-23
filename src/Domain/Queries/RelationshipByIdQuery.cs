using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class RelationshipByIdQuery : IQuery<WordRelation>
    {
        public long Id { get; set; }
    }
}