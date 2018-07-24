namespace Inshapardaz.Domain.Entities.Library
{
    public class ChapterContent
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public int ChapterId { get; set; }

        public string Content { get; set; }
    }
}