using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inshapardaz.Model
{
    public class DictionariesView
    {
        public IEnumerable<LinkView> Links { get; set; }

        public IEnumerable<DictionaryView> Items { get; set; }
    }
}
