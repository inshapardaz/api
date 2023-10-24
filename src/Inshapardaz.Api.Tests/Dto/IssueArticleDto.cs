using Inshapardaz.Domain.Models;
using System;

namespace Inshapardaz.Api.Tests.Dto
{
    public class IssueArticleDto
    {
        public IssueArticleDto()
        {
        }

        public IssueArticleDto(IssueArticleDto source)
        {
            Id = source.Id;
            IssueId = source.IssueId;
            Title = source.Title;
            SequenceNumber = source.SequenceNumber;
            SeriesName = source.SeriesName;
            SeriesIndex = source.SeriesIndex;
            Status = source.Status;
            WriterAccountId = source.WriterAccountId;
            WriterAssignTimeStamp = source.WriterAssignTimeStamp;
            ReviewerAccountId = source.ReviewerAccountId;
            ReviewerAssignTimeStamp = source.ReviewerAssignTimeStamp;
        }

        public int Id { get; set; }

        public int IssueId { get; set; }

        public string Title { get; set; }

        public int SequenceNumber { get; set; }
        public string SeriesName { get; set; }
        public int SeriesIndex { get; set; }

        public EditingStatus Status { get; set; }
        public int? WriterAccountId { get; set; }
        public DateTime? WriterAssignTimeStamp { get; set; }
        public int? ReviewerAccountId { get; set; }
        public DateTime? ReviewerAssignTimeStamp { get; set; }

    }

    public class IssueArticleContentDto
    {
        public int Id { get; set; }

        public int ArticleId { get; set; }

        public string Language { get; set; }

        public string Text { get; set; }
    }
}
