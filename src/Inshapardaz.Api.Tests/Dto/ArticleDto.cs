using Inshapardaz.Domain.Models;
using System;

namespace Inshapardaz.Api.Tests.Dto
{
    public class ArticleDto
    {
        public ArticleDto()
        {
        }

        public ArticleDto(ArticleDto source)
        {
            Id = source.Id;
            IssueId = source.IssueId;
            Title = source.Title;
            SequenceNumber = source.SequenceNumber;
            SeriesName = source.SeriesName;
            SeriesIndex = source.SeriesIndex;
            Status = source.Status;
            WriterAccountId = source.WriterAccountId;
            WriterAssignTimestamp = source.WriterAssignTimestamp;
            ReviewerAccountId = source.ReviewerAccountId;
            ReviewerAssignTimestamp = source.ReviewerAssignTimestamp;
        }

        public int Id { get; set; }

        public int IssueId { get; set; }

        public string Title { get; set; }

        public int SequenceNumber { get; set; }
        public string SeriesName { get; set; }
        public int SeriesIndex { get; set; }

        public EditingStatus Status { get; set; }
        public int? WriterAccountId { get; set; }
        public DateTime? WriterAssignTimestamp { get; set; }
        public int? ReviewerAccountId { get; set; }
        public DateTime? ReviewerAssignTimestamp { get; set; }

    }
}
