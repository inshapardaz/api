namespace Inshapardaz.Domain.Models.Library
{
    public class BookPageModel
    {
        public string Text { get; set; }
        public int PageNumber { get; set; }
        public int BookId { get; set; }
        public int? ImageId { get; set; }
    }
}
