using System;

namespace Inshapardaz.Domain.Models.Library
{
    public class IssuePageModel
    {
        public string Text { get; set; }
        public int SequenceNumber { get; set; }
        public int PeriodicalId { get; set; }
        public int VolumeNumber { get; set; }
        public int IssueNumber { get; set; }
        public long? ImageId { get; set; }
        public string ImageUrl { get; set; }
        public EditingStatus Status { get; set; }
        public int? WriterAccountId { get; set; }
        public string WriterAccountName { get; set; }
        public DateTime? WriterAssignTimeStamp { get; set; }
        public int? ReviewerAccountId { get; set; }
        public string ReviewerAccountName { get; set; }
        public DateTime? ReviewerAssignTimeStamp { get; set; }
        public IssuePageModel PreviousPage { get; internal set; }
        public IssuePageModel NextPage { get; internal set; }
        public long? ArticleId { get; set; }
        public string ArticleName { get; set; }
    }
}
