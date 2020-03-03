using System.Collections.Generic;

namespace Inshapardaz.Domain.Models.Library
{
    public class ChapterModel
    {
        public int Id { get; set; }

        public int ChapterNumber { get; set; }

        public string Title { get; set; }

        public int BookId { get; set; }

        public List<ChapterContentModel> Contents { get; set; } = new List<ChapterContentModel>();
    }
}
