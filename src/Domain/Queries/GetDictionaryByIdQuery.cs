using Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetDictionaryByIdQuery : IQuery<Model.Dictionary>
    {
        public string UserId { get; set; }

        public int DictionaryId { get; set; }
    }
}