using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Tests.Dto
{
    public class BookPageDto
    {
        public BookPageDto()
        {
        }

        public BookPageDto(BookPageDto source)
        {
            Id = source.Id;
            BookId = source.BookId;
            Text = source.Text;
            SequenceNumber = source.SequenceNumber;
            ImageId = source.ImageId;
            Status = source.Status;
            AccountId = source.AccountId;
        }

        public long Id { get; set; }

        public int BookId { get; set; }

        public string Text { get; set; }

        public int SequenceNumber { get; set; }

        public int? ImageId { get; set; }

        public EditingStatus Status { get; set; }
        public int? AccountId { get; set; }
    }
}
