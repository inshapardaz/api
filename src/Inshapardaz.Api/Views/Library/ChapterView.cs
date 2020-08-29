using System.Collections.Generic;

namespace Inshapardaz.Api.Views.Library
{
    public class ChapterView : ViewWithLinks
    {
        public int Id { get; set; }

        public int ChapterNumber { get; set; }

        public string Title { get; set; }

        public int BookId { get; set; }

        public IEnumerable<ChapterContentView> Contents { get; set; }
    }
}