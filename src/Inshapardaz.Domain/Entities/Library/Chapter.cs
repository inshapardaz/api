namespace Inshapardaz.Domain.Entities.Library
{
    public class Chapter
    {
        public int Id { get; set; }

        public uint ChapterNumber { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int BookId { get; set; }
    }
}