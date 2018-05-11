using System.Collections.Generic;

namespace Inshapardaz.Ports.Database.Entities
{
    public class Page<T>
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalCount { get; set; }

        public IEnumerable<T> Data { get; set; }
    }
}
