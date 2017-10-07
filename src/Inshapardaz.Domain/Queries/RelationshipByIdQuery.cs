using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class RelationshipByIdQuery : IQuery<WordRelation>
    {
        public long Id { get; set; }
    }
}