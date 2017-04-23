using System.Collections.Generic;
using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class RelationshipByWordIdQuery : IQuery<IEnumerable<WordRelation>>
    {
        public int WordId { get; set; }
    }
}