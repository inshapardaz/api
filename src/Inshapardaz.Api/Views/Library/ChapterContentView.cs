namespace Inshapardaz.Api.Views.Library
{
    public class ChapterContentView : ViewWithLinks
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public int ChapterId { get; set; }

        public int ChapterNumber { get; set; }

        public string Language { get; set; }

        public string MimeType { get; set; }
    }
}
