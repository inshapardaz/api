using Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class WordDetailByIdQuery : IQuery<WordDetail>
    {
        public long Id { get; set; }
    }
}