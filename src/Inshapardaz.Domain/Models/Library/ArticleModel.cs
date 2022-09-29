using System;
using System.Collections.Generic;

namespace Inshapardaz.Domain.Models.Library
{
    public class ArticleModel
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public int SequenceNumber { get; set; }

        public string SeriesName { get; set; }

        public int? SeriesIndex { get; set; }

        public List<AuthorModel> Authors { get; set; } = new List<AuthorModel>();

        public int IssueId { get; set; }

        public List<ArticleContentModel> Contents { get; set; } = new List<ArticleContentModel>();

        public int? WriterAccountId { get; set; }
        public string WriterAccountName { get; set; }
        public DateTime? WriterAssignTimeStamp { get; set; }
        public int? ReviewerAccountId { get; set; }
        public string ReviewerAccountName { get; set; }
        public DateTime? ReviewerAssignTimeStamp { get; set; }
        public EditingStatus Status { get; set; }
        public ArticleModel PreviousArticle { get; set; }
        public ArticleModel NextArticle { get; set; }
    }
}
