using Darker;

namespace Inshapardaz.Domain.Queries
{
    public class DictionaryByIdQuery : IQuery<Model.Dictionary>
    {
        public string UserId { get; set; }

        public int DictionaryId { get; set; }
    }
}