using System.Collections.Generic;
using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class GetWordMeaningsByWordQuery : IQuery<IEnumerable<Meaning>>
    {
        public GetWordMeaningsByWordQuery(long wordId)
        {
            WordId = wordId;
        }

        public long WordId { get; }
    }
}