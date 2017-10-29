using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class GetRelationshipByIdQuery : IQuery<WordRelation>
    {
        public long Id { get; set; }
    }
}