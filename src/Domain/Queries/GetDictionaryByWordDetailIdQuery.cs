using Darker;

namespace Inshapardaz.Domain.Queries
{
    public class DictionaryByWordDetailIdQuery : IQuery<Model.Dictionary>
    {
        public long WordDetailId { get; set; }
    }
}