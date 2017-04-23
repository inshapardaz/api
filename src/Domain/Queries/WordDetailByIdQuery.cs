using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class WordDetailByIdQuery : IQuery<WordDetail>
    {
        public long Id { get; set; }
    }
}