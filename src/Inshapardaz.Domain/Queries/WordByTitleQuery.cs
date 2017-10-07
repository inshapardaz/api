using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class WordByTitleQuery : IQuery<Word>
    {
        public string Title { get; set; }
        public string UserId { get; set; }
    }
}