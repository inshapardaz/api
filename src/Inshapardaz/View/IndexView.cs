using System.Collections.Generic;

namespace Inshapardaz.Api.View
{
    public class IndexView
    {
        public IEnumerable<LinkView> Links { get; set; }

        public IEnumerable<LinkView> Indexes { get; set; }
    }
}
