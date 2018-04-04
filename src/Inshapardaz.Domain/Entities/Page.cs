using System.Collections.Generic;

namespace Inshapardaz.Domain.Entities
{
    public class Page<T>
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public long TotalCount { get; set; }

        public IEnumerable<T> Data { get; set; }
    }
}
