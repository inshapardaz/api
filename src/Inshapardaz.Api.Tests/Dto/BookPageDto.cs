namespace Inshapardaz.Api.Tests.Dto
{
    public class BookPageDto
    {
        public long Id { get; set; }

        public int BookId { get; set; }

        public string Text { get; set; }

        public int PageNumber { get; set; }

        public int? ImageId { get; set; }
    }
}
