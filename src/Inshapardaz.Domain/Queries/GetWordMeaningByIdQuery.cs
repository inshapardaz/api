using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class GetWordMeaningByIdQuery : IQuery<Meaning>
    {
        public GetWordMeaningByIdQuery(long meaningId)
        {
            MeaningId = meaningId;
        }

        public long MeaningId { get; }
    }
}