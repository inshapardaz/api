using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class WordMeaningByIdQuery : IQuery<Meaning>
    {
        public long Id { get; set; }
    }
}