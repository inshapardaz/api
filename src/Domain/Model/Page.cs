using System.Collections.Generic;

namespace Inshapardaz.Domain.Model
{
    public class Page<T>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public IEnumerable<T> Data { get; set; }
    }
}
