using Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetDictionaryByWordIdQuery : IQuery<Model.Dictionary>
    {
        public int WordId { get; set; }
    }
}