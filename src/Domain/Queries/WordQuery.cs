using Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class WordQuery : IQuery<Page<Word>>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}