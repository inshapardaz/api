using System.Collections.Generic;

namespace Inshapardaz.Api.View.Library
{
    public class ChapterContentView
    {
        public int Id { get; set; }

        public int ChapterId { get; set; }

        public string Contents { get; set; }

        public IEnumerable<LinkView> Links { get; set; }
    }
}
