namespace Inshapardaz.Domain.Models.Library
{
    public class BookContentModel
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public string ContentUrl { get; set; }

        public string MimeType { get; set; }

        public string Language { get; set; }

        public string FileName { get; set; }

        public int FileId { get; internal set; }
    }
}
