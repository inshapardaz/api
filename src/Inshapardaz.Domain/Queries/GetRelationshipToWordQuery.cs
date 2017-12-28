using System.Collections.Generic;
using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

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