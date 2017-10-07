using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class WordRelationsByTitleQuery : IQuery<Page<Word>>
    {
        public string Title { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}