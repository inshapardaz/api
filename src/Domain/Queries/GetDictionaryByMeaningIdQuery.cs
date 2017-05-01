using Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetDictionaryByMeaningIdQuery : IQuery<Model.Dictionary>
    {
        public int MeaningId { get; set; }
    }
}