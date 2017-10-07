using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class DictionaryByWordIdQuery : IQuery<Dictionary>
    {
        public long WordId { get; set; }
    }
}