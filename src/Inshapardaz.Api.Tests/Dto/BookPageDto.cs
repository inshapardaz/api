using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Tests.Dto
{
    public class IssuePageDto
    {
        public IssuePageDto()
        {
        }

        public IssuePageDto(IssuePageDto source)
        {
            Id = source.Id;
            IssueId = source.IssueId;
            Text = source.Text;
            SequenceNumber = source.SequenceNumber;
            ImageId = source.ImageId;
            Status = source.Status;
            WriterAccountId = source.WriterAccountId;
            ReviewerAccountId = source.ReviewerAccountId;
        }

        public long Id { get; set; }

        public int IssueId { get; set; }

        public string Text { get; set; }

        public int SequenceNumber { get; set; }

        public int? ImageId { get; set; }

        public EditingStatus Status { get; set; }
        public int? WriterAccountId { get; set; }
        public int? ReviewerAccountId { get; set; }
    }
}
