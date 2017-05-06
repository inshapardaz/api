using System.Collections.Generic;
using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class RelationshipByWordIdQuery : IQuery<IEnumerable<WordRelation>>
    {
        public long WordId { get; set; }
    }
}