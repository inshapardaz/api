using System.Collections.Generic;
using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class GetRelationshipsByWordQuery : IQuery<IEnumerable<WordRelation>>
    {
        public GetRelationshipsByWordQuery(long wordId)
        {
            WordId = wordId;
        }

        public long WordId { get; }
    }
}