using Inshapardaz.Domain.Models;
using System;

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
            WriterAccountId = source.WriterAccountId;
            WriterAssignTimeStamp = source.WriterAssignTimeStamp;
            ReviewerAccountId = source.ReviewerAccountId;
            ReviewerAssignTimeStamp = source.ReviewerAssignTimeStamp;
        }

        public long Id { get; set; }

        public int BookId { get; set; }

        public string Text { get; set; }
        public int? ContentId { get; set; }

        public int SequenceNumber { get; set; }

        public int? ImageId { get; set; }

        public EditingStatus Status { get; set; }
        public int? WriterAccountId { get; set; }
        public DateTime? WriterAssignTimeStamp { get; set; }
        public int? ReviewerAccountId { get; set; }
        public DateTime? ReviewerAssignTimeStamp { get; set; }
        public object ChapterId { get; set; }
    }
}
