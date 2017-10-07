using Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class DictionaryByWordDetailIdQuery : IQuery<Dictionary>
    {
        public long WordDetailId { get; set; }
    }
}