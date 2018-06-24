using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries.Dictionary
{
    public class GetWordRelationsByTitleQuery : IQuery<Page<Word>>
    {
        public string Title { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}