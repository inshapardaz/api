using System.Collections.Generic;

namespace Inshapardaz.Api.Model
{
    public class IndexView
    {
        public IEnumerable<LinkView> Links { get; set; }

        public IEnumerable<LinkView> Indexes { get; set; }
    }
}
