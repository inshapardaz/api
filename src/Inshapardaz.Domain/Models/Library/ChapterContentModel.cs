namespace Inshapardaz.Domain.Models.Library
{
    public class ChapterContentModel
    {
        public long Id { get; set; }

        public int BookId { get; set; }

        public long ChapterId { get; set; }

        public int ChapterNumber { get; set; }

        public string Language { get; set; }

        public string Text { get; set; }
    }
}
