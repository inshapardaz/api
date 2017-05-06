using Darker;

namespace Inshapardaz.Domain.Queries
{
    public class DictionaryByMeaningIdQuery : IQuery<Model.Dictionary>
    {
        public long MeaningId { get; set; }
    }
}