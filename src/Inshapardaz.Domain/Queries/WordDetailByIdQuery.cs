using Inshapardaz.Domain.Database.Entities;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
{
    public class WordDetailByIdQuery : IQuery<WordDetail>
    {
        public long WordDetailId { get; set; }

        public int DictionaryId { get; set; }
    }
}