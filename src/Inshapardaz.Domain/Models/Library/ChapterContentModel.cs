namespace Inshapardaz.Domain.Models.Library
{
    public class ChapterContentModel
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public int ChapterId { get; set; }

        public int ChapterNumber { get; set; }

        public string Language { get; set; }

        public string Text { get; set; }
    }
}
