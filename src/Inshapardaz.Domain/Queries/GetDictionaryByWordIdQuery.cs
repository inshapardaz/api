using Inshapardaz.Domain.Entities;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
{
    public class DictionaryByWordIdQuery : IQuery<Dictionary>
    {
        public DictionaryByWordIdQuery(long wordId)
        {
            WordId = wordId;
        }

        public long WordId { get; }
    }
}