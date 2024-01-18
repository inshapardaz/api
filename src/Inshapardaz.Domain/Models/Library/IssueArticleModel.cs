using System;
using System.Collections.Generic;

namespace Inshapardaz.Domain.Models.Library
{
    public class IssueArticleModel
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public int SequenceNumber { get; set; }

        public string SeriesName { get; set; }

        public int? SeriesIndex { get; set; }

        public List<AuthorModel> Authors { get; set; } = new List<AuthorModel>();

        public int IssueId { get; set; }

        public List<IssueArticleContentModel> Contents { get; set; } = new List<IssueArticleContentModel>();

        public int? WriterAccountId { get; set; }
        public string WriterAccountName { get; set; }
        public DateTime? WriterAssignTimeStamp { get; set; }
        public int? ReviewerAccountId { get; set; }
        public string ReviewerAccountName { get; set; }
        public DateTime? ReviewerAssignTimeStamp { get; set; }
        public EditingStatus Status { get; set; }
        public IssueArticleModel PreviousArticle { get; set; }
        public IssueArticleModel NextArticle { get; set; }
    }
}
