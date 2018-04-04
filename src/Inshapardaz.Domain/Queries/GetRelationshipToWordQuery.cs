using System.Collections.Generic;
using Inshapardaz.Domain.Entities;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetRelationshipToWordQuery : IQuery<IEnumerable<WordRelation>>
    {
        public GetRelationshipToWordQuery(long wordId)
        {
            WordId = wordId;
        }

        public long WordId { get; }
    }
}