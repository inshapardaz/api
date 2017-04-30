using System.Collections.Generic;

namespace Inshapardaz.Api.Model
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
