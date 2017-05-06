using Darker;

namespace Inshapardaz.Domain.Queries
{
    public class DictionaryByWordIdQuery : IQuery<Model.Dictionary>
    {
        public long WordId { get; set; }
    }
}