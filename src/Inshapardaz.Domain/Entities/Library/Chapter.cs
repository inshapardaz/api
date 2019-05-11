using System.Collections.Generic;

namespace Inshapardaz.Domain.Entities.Library
{
    public class Chapter
    {
        public int Id { get; set; }

        public int ChapterNumber { get; set; }

        public string Title { get; set; }

        public int BookId { get; set; }
        
        public IEnumerable<ChapterContent> Contents { get; set; }
    }
}