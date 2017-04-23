using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class WordByIdQuery : IQuery<Word>
    {
        public int Id { get; set; }

        public string UserId { get; set; }
    }
}