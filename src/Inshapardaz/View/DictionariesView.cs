using System.Collections.Generic;

namespace Inshapardaz.Api.View
{
    public class DictionariesView
    {
        public IEnumerable<LinkView> Links { get; set; }

        public IEnumerable<DictionaryView> Items { get; set; }
    }
}
