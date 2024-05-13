namespace Inshapardaz.Api.Views.Library
{
    public class BookContentView : ViewWithLinks
    {
        public long Id { get; set; }

        public int BookId { get; set; }

        public string FileName { get; set; } 

        public string MimeType { get; set; }

        public string Language { get; set; }
    }
}
