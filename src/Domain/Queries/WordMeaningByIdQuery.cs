using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class WordMeaningByIdQuery : IQuery<Meaning>
    {
        public int Id { get; set; }
    }
}