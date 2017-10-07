using Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class DictionaryByMeaningIdQuery : IQuery<Dictionary>
    {
        public long MeaningId { get; set; }
    }
}