using Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetDictionaryByWordDetailIdQuery : IQuery<Model.Dictionary>
    {
        public int WordDetailId { get; set; }
    }
}