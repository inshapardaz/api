using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inshapardaz.Model
{
    public class DictionaryView
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Language { get; set; }

        public string UserId { get; set;  }

        public bool IsPublic { get; set; }

        public long WordCount { get; set; }

        public IEnumerable<LinkView> Links { get; set; }
    }
}
