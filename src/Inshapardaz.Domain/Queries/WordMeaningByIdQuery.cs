using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class WordMeaningByIdQuery : IQuery<Meaning>
    {
        public long Id { get; set; }
    }
}