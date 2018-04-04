using Inshapardaz.Domain.Entities;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetDictionaryByIdQuery : IQuery<Dictionary>
    {

        public int DictionaryId { get; set; }
    }
}